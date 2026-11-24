import { useState, useEffect } from "react";
import { authFetch } from "./api";

function Books({ isAdmin, isMember, onAuthError }) {
  const [books, setBooks] = useState([]);
  const [search, setSearch] = useState("");
  const [available, setAvailable] = useState("all");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [info, setInfo] = useState("");
  const [editingId, setEditingId] = useState(null);
  const [editTitle, setEditTitle] = useState("");
  const [editAuthor, setEditAuthor] = useState("");

  const [newTitle, setNewTitle] = useState("");
  const [newAuthor, setNewAuthor] = useState("");

  const showActions = isAdmin || isMember;

  async function loadBooks() {
    setLoading(true);
    setError("");
    const params = new URLSearchParams();
    if (search) params.append("search", search);
    if (available !== "all") params.append("available", available);
    try {
      const response = await authFetch(`/api/books?${params.toString()}`);
      if (response.status === 401) { onAuthError(); return; }
      if (!response.ok) { setError("Failed to load books."); return; }
      const data = await response.json();
      setBooks(data);
    } catch (err) {
      setError("Could not reach the server.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadBooks(); }, []);

  function handleSearch(e) {
    e.preventDefault();
    loadBooks();
  }

  async function addBook(e) {
    e.preventDefault();
    try {
      const response = await authFetch(`/api/books`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title: newTitle, author: newAuthor })
      });
      if (response.status === 401) { onAuthError(); return; }
      if (response.status === 403) { setError("Only admins can add books."); return; }
      if (!response.ok) { setError("Failed to add book."); return; }
      setNewTitle("");
      setNewAuthor("");
      loadBooks();
    } catch (err) {
      setError("Could not reach the server.");
    }
  }

  async function deleteBook(id) {
    if (!window.confirm("Delete this book?")) return;
    try {
      const response = await authFetch(`/api/books/${id}`, { method: "DELETE" });
      if (response.status === 401) { onAuthError(); return; }
      if (response.status === 403) { setError("Only admins can delete books."); return; }
      if (!response.ok) { setError("Failed to delete book."); return; }
      loadBooks();
    } catch (err) {
      setError("Could not reach the server.");
    }
  }

  function startEdit(book) {
    setEditingId(book.id);
    setEditTitle(book.title);
    setEditAuthor(book.author);
    setError("");
  }

  function cancelEdit() {
    setEditingId(null);
  }

  async function saveEdit(book) {
    try {
      const response = await authFetch(`/api/books/${book.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          id: book.id,
          title: editTitle,
          author: editAuthor,
          isAvailable: book.isAvailable
        })
      });
      if (response.status === 401) { onAuthError(); return; }
      if (response.status === 403) { setError("Only admins can edit books."); return; }
      if (!response.ok) { setError("Failed to update book."); return; }
      setEditingId(null);
      loadBooks();
    } catch (err) {
      setError("Could not reach the server.");
    }
  }

  // Member: request to borrow a book -> goes to an admin for approval
  async function requestBook(book) {
    setError("");
    setInfo("");
    try {
      const response = await authFetch(`/api/bookrequests`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ bookId: book.id })
      });
      if (response.status === 401) { onAuthError(); return; }
      if (!response.ok) {
        const msg = await response.text();
        setError(msg || "Failed to request book.");
        return;
      }
      setInfo(`Requested "${book.title}". An admin will review it.`);
    } catch (err) {
      setError("Could not reach the server.");
    }
  }

  return (
    <div>
      {isAdmin && (
        <form className="row g-2 mb-4 p-3 bg-light rounded border" onSubmit={addBook}>
          <div className="col-12"><strong>Add a new book</strong></div>
          <div className="col-md-5">
            <input className="form-control" placeholder="Title"
              value={newTitle} onChange={(e) => setNewTitle(e.target.value)} required />
          </div>
          <div className="col-md-5">
            <input className="form-control" placeholder="Author"
              value={newAuthor} onChange={(e) => setNewAuthor(e.target.value)} required />
          </div>
          <div className="col-md-2">
            <button className="btn btn-success w-100" type="submit">Add</button>
          </div>
        </form>
      )}

      <form className="row g-2 mb-3" onSubmit={handleSearch}>
        <div className="col-md-6">
          <input className="form-control" placeholder="Search by title or author..."
            value={search} onChange={(e) => setSearch(e.target.value)} />
        </div>
        <div className="col-md-3">
          <select className="form-select" value={available}
            onChange={(e) => setAvailable(e.target.value)}>
            <option value="all">All books</option>
            <option value="true">Available only</option>
            <option value="false">Borrowed only</option>
          </select>
        </div>
        <div className="col-md-3">
          <button className="btn btn-primary w-100" type="submit">Search</button>
        </div>
      </form>

      {loading && <p className="text-muted">Loading...</p>}
      {error && <div className="alert alert-danger">{error}</div>}
      {info && <div className="alert alert-success">{info}</div>}

      {!loading && !error && (
        <table className="table table-striped table-hover align-middle">
          <thead>
            <tr>
              <th>#</th>
              <th>Title</th>
              <th>Author</th>
              <th>Status</th>
              {showActions && <th>Actions</th>}
            </tr>
          </thead>
          <tbody>
            {books.length === 0 ? (
              <tr>
                <td colSpan={showActions ? 5 : 4} className="text-center text-muted">
                  No books found.
                </td>
              </tr>
            ) : (
              books.map((book) => (
                <tr key={book.id}>
                  <td>{book.id}</td>

                  {editingId === book.id ? (
                    <>
                      <td>
                        <input className="form-control form-control-sm"
                          value={editTitle} onChange={(e) => setEditTitle(e.target.value)} />
                      </td>
                      <td>
                        <input className="form-control form-control-sm"
                          value={editAuthor} onChange={(e) => setEditAuthor(e.target.value)} />
                      </td>
                    </>
                  ) : (
                    <>
                      <td>{book.title}</td>
                      <td>{book.author}</td>
                    </>
                  )}

                  <td>
                    {book.isAvailable
                      ? <span className="badge bg-success">Available</span>
                      : <span className="badge bg-secondary">Borrowed</span>}
                  </td>

                  {showActions && (
                    <td>
                      {isAdmin && (editingId === book.id ? (
                        <>
                          <button className="btn btn-success btn-sm me-1"
                            onClick={() => saveEdit(book)}>Save</button>
                          <button className="btn btn-secondary btn-sm"
                            onClick={cancelEdit}>Cancel</button>
                        </>
                      ) : (
                        <>
                          <button className="btn btn-outline-primary btn-sm me-1"
                            onClick={() => startEdit(book)}>Edit</button>
                          <button className="btn btn-outline-danger btn-sm"
                            onClick={() => deleteBook(book.id)}>Delete</button>
                        </>
                      ))}

                      {isMember && (
                        book.isAvailable
                          ? <button className="btn btn-outline-success btn-sm"
                              onClick={() => requestBook(book)}>Request</button>
                          : <span className="text-muted small">Unavailable</span>
                      )}
                    </td>
                  )}
                </tr>
              ))
            )}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default Books;
