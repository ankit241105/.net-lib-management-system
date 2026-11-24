// Central API helper: holds the access token, talks to the auth endpoints, and wraps
// fetch so an expired access token is silently refreshed (via the httpOnly refresh cookie)
// and the original request is retried once.
export const API = "http://localhost:5016";

export function getToken() {
  return localStorage.getItem("token");
}
function setToken(t) {
  localStorage.setItem("token", t);
}
function clearLocal() {
  localStorage.removeItem("token");
  localStorage.removeItem("username");
}

export async function login(username, password) {
  const res = await fetch(`${API}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",            // so the Set-Cookie refresh token is stored
    body: JSON.stringify({ username, password })
  });
  if (!res.ok) return false;
  const data = await res.json();       // { token }
  setToken(data.token);
  localStorage.setItem("username", username);
  return true;
}

export async function register(username, email, password) {
  const res = await fetch(`${API}/api/auth/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, email, password })
  });
  let message = "";
  try { message = (await res.text()).replace(/^"|"$/g, ""); } catch { /* ignore */ }
  return { ok: res.ok, message };
}

export async function logout() {
  try {
    await fetch(`${API}/api/auth/logout`, { method: "POST", credentials: "include" });
  } catch {
    /* even if the server call fails, clear local state */
  }
  clearLocal();
}

// De-duplicate concurrent refreshes: many 401s at once share one refresh call.
let refreshing = null;
function refreshAccess() {
  if (!refreshing) {
    refreshing = fetch(`${API}/api/auth/refresh`, { method: "POST", credentials: "include" })
      .then(async (res) => {
        if (!res.ok) return false;
        const data = await res.json();
        setToken(data.token);
        return true;
      })
      .catch(() => false)
      .finally(() => { refreshing = null; });
  }
  return refreshing;
}

// Authenticated fetch. `path` starts with "/api/...". On a 401 it tries one refresh and
// retries; if that fails the 401 is returned so the caller can send the user to login.
export async function authFetch(path, options = {}) {
  const run = () =>
    fetch(`${API}${path}`, {
      ...options,
      headers: { ...(options.headers || {}), Authorization: `Bearer ${getToken()}` }
    });

  let res = await run();
  if (res.status === 401) {
    const ok = await refreshAccess();
    if (ok) res = await run();
  }
  return res;
}
