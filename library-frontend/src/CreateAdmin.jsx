import { useState } from "react";
import { authFetch } from "./api";

function CreateAdmin({ onAuthError }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  async function handleSubmit(e) {
    e.preventDefault();
    setMessage("");
    setError("");
    try {
      const response = await authFetch(`/api/auth/register-admin`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
      });
      if (response.status === 401) { onAuthError(); return; }
      if (response.status === 403) { setError("Only admins can create admins."); return; }
      if (!response.ok) {
        const msg = await response.text();
        setError(msg || "Failed to create admin.");
        return;
      }
      const text = await response.text();
      setMessage(text || "Admin created successfully.");
      setUsername("");
      setPassword("");
    } catch (err) {
      setError("Could not reach the server.");
    }
  }

  return (
    <div className="container" style={{ maxWidth: "500px" }}>
      <div className="card shadow-sm">
        <div className="card-body">
          <h4 className="card-title mb-1">Create a new Admin</h4>
          <p className="text-muted small">Only admins can create other admin accounts.</p>
          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label">Username</label>
              <input className="form-control" value={username}
                onChange={(e) => setUsername(e.target.value)} required />
            </div>
            <div className="mb-3">
              <label className="form-label">Password</label>
              <input type="password" className="form-control" value={password}
                onChange={(e) => setPassword(e.target.value)} required />
            </div>
            {error && <div className="alert alert-danger py-2">{error}</div>}
            {message && <div className="alert alert-success py-2">{message}</div>}
            <button type="submit" className="btn btn-primary">Create Admin</button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default CreateAdmin;
