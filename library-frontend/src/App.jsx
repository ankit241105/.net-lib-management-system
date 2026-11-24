import { useState } from "react";
import { logout as apiLogout, getToken } from "./api";
import Login from "./Login";
import Register from "./Register";
import Books from "./Books";
import Borrowing from "./Borrowing";
import Dashboard from "./Dashboard";
import CreateAdmin from "./CreateAdmin";
import Approvals from "./Approvals";
import UpgradeBanner from "./UpgradeBanner";

function getRole(token) {
  if (!token) return null;
  try {
    const base64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
    const payload = JSON.parse(atob(base64));
    return (
      payload["role"] ||
      payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
      null
    );
  } catch {
    return null;
  }
}

function App() {
  const [token, setToken] = useState(getToken());
  const [username, setUsername] = useState(localStorage.getItem("username"));
  const [page, setPage] = useState("books");
  const [authView, setAuthView] = useState("login");   // "login" | "register" when logged out

  const role = getRole(token);
  const isAdmin = role === "Admin";
  const isMember = role === "Member";
  const isUser = role === "User";

  // Login already stored the token + username (in api.login); just sync state.
  function handleLogin() {
    setToken(getToken());
    setUsername(localStorage.getItem("username"));
    setPage("books");
  }

  async function handleLogout() {
    await apiLogout();   // revokes the refresh token server-side + clears local state
    setToken(null);
    setUsername(null);
  }

  if (!token) {
    return authView === "register"
      ? <Register onBackToLogin={() => setAuthView("login")} />
      : <Login onLogin={handleLogin} onCreateAccount={() => setAuthView("register")} />;
  }

  function Tab({ id, children }) {
    return (
      <li className="nav-item">
        <button
          className={"nav-link " + (page === id ? "active" : "")}
          onClick={() => setPage(id)}
        >
          {children}
        </button>
      </li>
    );
  }

  return (
    <div className="container py-4">
      <div className="d-flex justify-content-between align-items-center mb-3 border-bottom pb-3">
        <h1 className="h3 text-primary mb-0">📚 Library Management</h1>
        <div>
          <span className="me-3">
            Signed in as <strong>{username}</strong>
            {role && (
              <span className={"badge ms-2 " + (isAdmin ? "bg-warning text-dark" : isMember ? "bg-success" : "bg-secondary")}>
                {role}
              </span>
            )}
          </span>
          <button className="btn btn-outline-danger btn-sm" onClick={handleLogout}>
            Logout
          </button>
        </div>
      </div>

      {isUser && <UpgradeBanner token={token} onAuthError={handleLogout} />}

      <ul className="nav nav-pills mb-4">
        <Tab id="books">Books</Tab>
        {isAdmin && <Tab id="borrowing">Loans</Tab>}
        {isMember && <Tab id="borrowing">My Books</Tab>}
        {isAdmin && <Tab id="approvals">Approvals</Tab>}
        {isAdmin && <Tab id="dashboard">Dashboard</Tab>}
        {isAdmin && <Tab id="createAdmin">Create Admin</Tab>}
      </ul>

      {page === "books" && (
        <Books token={token} isAdmin={isAdmin} isMember={isMember} onAuthError={handleLogout} />
      )}
      {page === "borrowing" && (isAdmin || isMember) && (
        <Borrowing token={token} isAdmin={isAdmin} onAuthError={handleLogout} />
      )}
      {page === "approvals" && isAdmin && (
        <Approvals token={token} onAuthError={handleLogout} />
      )}
      {page === "dashboard" && isAdmin && (
        <Dashboard token={token} onAuthError={handleLogout} />
      )}
      {page === "createAdmin" && isAdmin && (
        <CreateAdmin token={token} onAuthError={handleLogout} />
      )}
    </div>
  );
}

export default App;
