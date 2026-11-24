import { useState, useEffect } from "react";
import { authFetch } from "./api";

// Role-aware:
//  - Admin  -> all loans, with a Return action.
//  - Member -> their own loans + their own book requests (read-only).
function Borrowing({ isAdmin, onAuthError }) {
  const [records, setRecords] = useState([]);
  const [myRequests, setMyRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  async function loadAll() {
    setLoading(true);
    setError("");
    try {
      if (isAdmin) {
        const res = await authFetch(`/api/borrowrecords`);
        if (res.status === 401) { onAuthError(); return; }
        if (!res.ok) { setError("Failed to load loans."); return; }
        setRecords(await res.json());
      } else {
        const [loanRes, reqRes] = await Promise.all([
          authFetch(`/api/borrowrecords/mine`),
          authFetch(`/api/bookrequests/mine`)
        ]);
        if (loanRes.status === 401 || reqRes.status === 401) { onAuthError(); return; }
        if (!loanRes.ok || !reqRes.ok) { setError("Failed to load your data."); return; }
        setRecords(await loanRes.json());
        setMyRequests(await reqRes.json());
      }
    } catch {
      setError("Could not reach the server.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadAll(); }, []);

  async function returnBook(id) {
    setError("");
    try {
      const res = await authFetch(`/api/borrowrecords/${id}/return`, { method: "PUT" });
      if (res.status === 401) { onAuthError(); return; }
      if (!res.ok) {
        const msg = await res.text();
        setError(msg || "Failed to return book.");
        return;
      }
      loadAll();
    } catch {
      setError("Could not reach the server.");
    }
  }

  const statusBadge = (s) => {
    const map = { Pending: "bg-warning text-dark", Approved: "bg-success", Declined: "bg-danger" };
    return <span className={`badge ${map[s] || "bg-secondary"}`}>{s}</span>;
  };

  const isOverdue = (r) => !r.returnDate && new Date(r.dueDate) < new Date();
  const fineCell = (r) =>
    r.fine > 0
      ? <span className="text-danger fw-bold">₹{r.fine}</span>
      : <span className="text-muted">—</span>;

  return (
    <div>
      {error && <div className="alert alert-danger">{error}</div>}
      {loading && <p className="text-muted">Loading...</p>}

      {/* Members: their pending/decided book requests */}
      {!isAdmin && !loading && (
        <>
          <h4 className="h5 mb-3">My requests</h4>
          <table className="table table-striped align-middle mb-5">
            <thead>
              <tr><th>#</th><th>Book</th><th>Status</th><th>Requested</th></tr>
            </thead>
            <tbody>
              {myRequests.length === 0 ? (
                <tr><td colSpan="4" className="text-center text-muted">No requests yet. Request a book from the Books tab.</td></tr>
              ) : (
                myRequests.map((r) => (
                  <tr key={r.id}>
                    <td>{r.id}</td>
                    <td>{r.bookTitle}</td>
                    <td>{statusBadge(r.status)}</td>
                    <td>{new Date(r.requestedDate).toLocaleDateString()}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </>
      )}

      {!loading && (
        <>
          <h4 className="h5 mb-3">{isAdmin ? "All loans" : "My borrowed books"}</h4>
          <table className="table table-striped table-hover align-middle">
            <thead>
              <tr>
                <th>#</th>
                <th>Book</th>
                {isAdmin && <th>Borrower</th>}
                <th>Borrowed On</th>
                <th>Due</th>
                <th>Status</th>
                <th>Fine</th>
                {isAdmin && <th>Action</th>}
              </tr>
            </thead>
            <tbody>
              {records.length === 0 ? (
                <tr>
                  <td colSpan={isAdmin ? 8 : 6} className="text-center text-muted">No borrow records yet.</td>
                </tr>
              ) : (
                records.map((r) => (
                  <tr key={r.id}>
                    <td>{r.id}</td>
                    <td>{r.book ? r.book.title : "(deleted)"}</td>
                    {isAdmin && <td>{r.username || "(unknown)"}</td>}
                    <td>{new Date(r.borrowDate).toLocaleDateString()}</td>
                    <td>{new Date(r.dueDate).toLocaleDateString()}</td>
                    <td>
                      {r.returnDate
                        ? <span className="badge bg-secondary">
                            Returned {new Date(r.returnDate).toLocaleDateString()}
                          </span>
                        : isOverdue(r)
                          ? <span className="badge bg-danger">Overdue</span>
                          : <span className="badge bg-warning text-dark">Borrowed</span>}
                    </td>
                    <td>{fineCell(r)}</td>
                    {isAdmin && (
                      <td>
                        {!r.returnDate && (
                          <button className="btn btn-outline-primary btn-sm"
                            onClick={() => returnBook(r.id)}>Return</button>
                        )}
                      </td>
                    )}
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </>
      )}
    </div>
  );
}

export default Borrowing;
