import { useState } from "react";
import { register } from "./api";

// Public self-registration. Creates a plain "User" (role decided server-side).
function Register({ onBackToLogin }) {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [done, setDone] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    try {
      const { ok, message } = await register(username, email, password);
      if (!ok) {
        setError(message || "Registration failed.");
        return;
      }
      setDone(true);
    } catch {
      setError("Could not reach the server.");
    }
  }

  return (
    <div className="container" style={{ maxWidth: "400px" }}>
      <div className="card shadow-sm mt-5">
        <div className="card-body">
          <h3 className="card-title text-center text-primary mb-4">📚 Create account</h3>

          {done ? (
            <>
              <div className="alert alert-success">
                Account created! You can now log in.
              </div>
              <button className="btn btn-primary w-100" onClick={onBackToLogin}>
                Go to login
              </button>
            </>
          ) : (
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label className="form-label">Username</label>
                <input className="form-control" value={username}
                  onChange={(e) => setUsername(e.target.value)} required />
              </div>
              <div className="mb-3">
                <label className="form-label">Email</label>
                <input type="email" className="form-control" value={email}
                  onChange={(e) => setEmail(e.target.value)} required />
                <div className="form-text">Used for due-date reminder emails.</div>
              </div>
              <div className="mb-3">
                <label className="form-label">Password</label>
                <input type="password" className="form-control" value={password}
                  onChange={(e) => setPassword(e.target.value)} required />
              </div>
              {error && <div className="alert alert-danger py-2">{error}</div>}
              <button type="submit" className="btn btn-primary w-100">Create account</button>
              <button type="button" className="btn btn-link w-100 mt-2" onClick={onBackToLogin}>
                Already have an account? Log in
              </button>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}

export default Register;
