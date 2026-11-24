import { useState, useEffect } from "react";
import { authFetch } from "./api";

// Shown only to plain "User" accounts: invites them to upgrade to Member for ₹199
// (mock payment). The upgrade is a request that an admin must approve.
function UpgradeBanner({ onAuthError }) {
  const [pending, setPending] = useState(false);
  const [error, setError] = useState("");

  async function loadStatus() {
    try {
      const res = await authFetch(`/api/membershiprequests/mine`);
      if (res.status === 401) { onAuthError(); return; }
      if (!res.ok) return;
      const data = await res.json();
      setPending(data.some((r) => r.status === "Pending"));
    } catch {
      /* banner is non-critical; ignore load errors */
    }
  }

  useEffect(() => { loadStatus(); }, []);

  async function requestUpgrade() {
    setError("");
    try {
      const res = await authFetch(`/api/membershiprequests`, { method: "POST" });
      if (res.status === 401) { onAuthError(); return; }
      if (!res.ok) {
        const msg = await res.text();
        setError(msg || "Could not submit upgrade request.");
        return;
      }
      setPending(true);
    } catch {
      setError("Could not reach the server.");
    }
  }

  if (pending) {
    return (
      <div className="alert alert-info d-flex justify-content-between align-items-center">
        <span>⏳ Your membership upgrade is <strong>pending admin approval</strong>. Once approved, log out and back in to borrow books.</span>
      </div>
    );
  }

  return (
    <div className="alert alert-warning d-flex justify-content-between align-items-center flex-wrap">
      <span>
        ⭐ <strong>Become a Member for ₹199</strong> and start requesting books to borrow.
      </span>
      <div className="d-flex align-items-center gap-2">
        {error && <span className="text-danger small">{error}</span>}
        <button className="btn btn-warning btn-sm" onClick={requestUpgrade}>
          Upgrade for ₹199
        </button>
      </div>
    </div>
  );
}

export default UpgradeBanner;
