import { useState, useEffect } from "react";
import { authFetch } from "./api";

function Dashboard({ onAuthError }) {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  async function loadStats() {
    setLoading(true);
    setError("");
    try {
      const response = await authFetch(`/api/dashboard`);
      if (response.status === 401) { onAuthError(); return; }
      if (response.status === 403) { setError("Admins only."); return; }
      if (!response.ok) { setError("Failed to load dashboard."); return; }
      const data = await response.json();
      setStats(data);
    } catch (err) {
      setError("Could not reach the server.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadStats(); }, []);

  if (loading) return <p className="text-muted">Loading...</p>;
  if (error) return <div className="alert alert-danger">{error}</div>;
  if (!stats) return null;

  // Catalog / loan stats
  const catalogCards = [
    { label: "Total Books", value: stats.totalBooks, color: "primary" },
    { label: "Available", value: stats.availableBooks, color: "success" },
    { label: "Currently Borrowed", value: stats.currentlyBorrowed, color: "warning" },
    { label: "Overdue", value: stats.overdueLoans, color: "danger" }
  ];

  // People stats (by role)
  const peopleCards = [
    { label: "Total Users", value: stats.totalUsers, color: "dark" },
    { label: "Regular Users", value: stats.totalRegularUsers, color: "secondary" },
    { label: "Members", value: stats.totalMembers, color: "info" },
    { label: "Admins", value: stats.totalAdmins, color: "primary" }
  ];

  // Items awaiting an admin decision (highlighted when there's something to do)
  const pending = [
    { label: "Book requests awaiting approval", value: stats.pendingBookRequests },
    { label: "Membership requests awaiting approval", value: stats.pendingMembershipRequests }
  ];

  const StatCard = ({ label, value, color }) => (
    <div className="col-6 col-md-3">
      <div className={`card text-bg-${color} shadow-sm`}>
        <div className="card-body text-center">
          <div className="display-6 fw-bold">{value}</div>
          <div>{label}</div>
        </div>
      </div>
    </div>
  );

  return (
    <div>
      {/* Pending actions */}
      <div className="row g-3 mb-4">
        {pending.map((p) => (
          <div className="col-12 col-md-6" key={p.label}>
            <div className={`card shadow-sm ${p.value > 0 ? "border-warning" : ""}`}>
              <div className="card-body d-flex align-items-center justify-content-between">
                <span className="fw-semibold">{p.label}</span>
                <span className={`badge fs-6 ${p.value > 0 ? "text-bg-warning" : "text-bg-secondary"}`}>
                  {p.value}
                </span>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Catalog stats */}
      <h6 className="text-muted text-uppercase small mb-2">Catalog</h6>
      <div className="row g-3 mb-4">
        {catalogCards.map((c) => <StatCard key={c.label} {...c} />)}
      </div>

      {/* People stats */}
      <h6 className="text-muted text-uppercase small mb-2">People</h6>
      <div className="row g-3 mb-4">
        {peopleCards.map((c) => <StatCard key={c.label} {...c} />)}
      </div>

      {/* Active borrowings */}
      <h5 className="mb-3">Currently Borrowed Books</h5>
      {stats.activeBorrowings.length === 0 ? (
        <p className="text-muted">No books are currently out.</p>
      ) : (
        <table className="table table-striped align-middle">
          <thead>
            <tr>
              <th>#</th>
              <th>Book</th>
              <th>Borrower</th>
              <th>Borrowed On</th>
              <th>Due</th>
              <th>Fine so far</th>
            </tr>
          </thead>
          <tbody>
            {stats.activeBorrowings.map((r) => {
              const overdue = new Date(r.dueDate) < new Date();
              return (
                <tr key={r.id} className={overdue ? "table-danger" : ""}>
                  <td>{r.id}</td>
                  <td>{r.book ? r.book.title : "(deleted)"}</td>
                  <td>{r.username || "(unknown)"}</td>
                  <td>{new Date(r.borrowDate).toLocaleDateString()}</td>
                  <td>{new Date(r.dueDate).toLocaleDateString()}</td>
                  <td>
                    {r.fine > 0
                      ? <span className="text-danger fw-bold">₹{r.fine}</span>
                      : <span className="text-muted">—</span>}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default Dashboard;
