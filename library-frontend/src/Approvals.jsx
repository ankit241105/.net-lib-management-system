import { useState, useEffect } from "react";
import { authFetch } from "./api";

// Admin-only: review pending book requests and membership-upgrade requests,
// approving or declining each.
function Approvals({ onAuthError }) {
  const [bookReqs, setBookReqs] = useState([]);
  const [memberReqs, setMemberReqs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  async function loadAll() {
    setLoading(true);
    setError("");
    try {
      const [brRes, mrRes] = await Promise.all([
        authFetch(`/api/bookrequests/pending`),
        authFetch(`/api/membershiprequests/pending`)
      ]);
      if (brRes.status === 401 || mrRes.status === 401) { onAuthError(); return; }
      if (!brRes.ok || !mrRes.ok) { setError("Failed to load requests."); return; }
      setBookReqs(await brRes.json());
      setMemberReqs(await mrRes.json());
    } catch {
      setError("Could not reach the server.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadAll(); }, []);

  async function decide(url) {
    setError("");
    try {
      const res = await authFetch(url, { method: "PUT" });
      if (res.status === 401) { onAuthError(); return; }
      if (!res.ok) {
        const msg = await res.text();
        setError(msg || "Action failed.");
        return;
      }
      loadAll();
    } catch {
      setError("Could not reach the server.");
    }
  }

  return (
    <div>
      {error && <div className="alert alert-danger">{error}</div>}
      {loading && <p className="text-muted">Loading...</p>}

      <h4 className="h5 mb-3">📖 Book requests</h4>
      <table className="table table-striped align-middle mb-5">
        <thead>
          <tr><th>#</th><th>Book</th><th>Requested by</th><th>On</th><th>Action</th></tr>
        </thead>
        <tbody>
          {bookReqs.length === 0 ? (
            <tr><td colSpan="5" className="text-center text-muted">No pending book requests.</td></tr>
          ) : (
            bookReqs.map((r) => (
              <tr key={r.id}>
                <td>{r.id}</td>
                <td>{r.bookTitle}</td>
                <td>{r.username}</td>
                <td>{new Date(r.requestedDate).toLocaleDateString()}</td>
                <td>
                  <button className="btn btn-success btn-sm me-1"
                    onClick={() => decide(`/api/bookrequests/${r.id}/approve`)}>Approve</button>
                  <button className="btn btn-outline-danger btn-sm"
                    onClick={() => decide(`/api/bookrequests/${r.id}/decline`)}>Decline</button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      <h4 className="h5 mb-3">⭐ Membership upgrade requests</h4>
      <table className="table table-striped align-middle">
        <thead>
          <tr><th>#</th><th>User</th><th>Amount</th><th>On</th><th>Action</th></tr>
        </thead>
        <tbody>
          {memberReqs.length === 0 ? (
            <tr><td colSpan="5" className="text-center text-muted">No pending membership requests.</td></tr>
          ) : (
            memberReqs.map((r) => (
              <tr key={r.id}>
                <td>{r.id}</td>
                <td>{r.username}</td>
                <td>₹{r.amount}</td>
                <td>{new Date(r.requestedDate).toLocaleDateString()}</td>
                <td>
                  <button className="btn btn-success btn-sm me-1"
                    onClick={() => decide(`/api/membershiprequests/${r.id}/approve`)}>Approve</button>
                  <button className="btn btn-outline-danger btn-sm"
                    onClick={() => decide(`/api/membershiprequests/${r.id}/decline`)}>Decline</button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}

export default Approvals;
