import { useState } from "react";
import { login } from "./api";

function Login({ onLogin, onCreateAccount }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    try {
      const ok = await login(username, password);   // stores access token + sets refresh cookie
      if (!ok) {
        setError("Invalid username or password.");
        return;
      }
      onLogin();
    } catch (err) {
      setError("Could not reach the server.");
    }
  }

  return (
    <div className="container" style={{ maxWidth: "400px" }}>
      <div className="card shadow-sm mt-5">
        <div className="card-body">
          <h3 className="card-title text-center text-primary mb-4">📚 Library Login</h3>
          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label">Username</label>
              <input
                className="form-control"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
            </div>
            <div className="mb-3">
              <label className="form-label">Password</label>
              <input
                type="password"
                className="form-control"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
            {error && <div className="alert alert-danger py-2">{error}</div>}
            <button type="submit" className="btn btn-primary w-100">Log in</button>
            <button type="button" className="btn btn-link w-100 mt-2" onClick={onCreateAccount}>
              New here? Create an account
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Login;