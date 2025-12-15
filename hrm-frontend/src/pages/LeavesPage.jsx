import { useEffect, useState } from "react";

const API_BASE = "http://localhost:5000";
const DEFAULT_BRANCH_ID = "";

function LeavesPage() {
  const [leaves, setLeaves] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);
        setError(null);
        const res = await fetch(`${API_BASE}/api/leaverequests`, {
          headers: {
            "X-Branch-Id": DEFAULT_BRANCH_ID,
          },
        });
        if (!res.ok) {
          throw new Error(`API error: ${res.status}`);
        }
        const data = await res.json();
        setLeaves(data);
      } catch (err) {
        setError(err.message || "Failed to load leave requests");
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []);

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-800">Leave Requests</h2>
        <button className="px-3 py-2 text-sm font-medium rounded-md bg-slate-900 text-white hover:bg-slate-800">
          New Leave Request
        </button>
      </div>

      {loading && <p className="text-sm text-slate-500">Loading leave requests...</p>}
      {error && (
        <p className="text-sm text-red-600">
          {error} (ensure API is running on {API_BASE} and X-Branch-Id is valid)
        </p>
      )}

      {!loading && !error && (
        <div className="overflow-hidden rounded-lg border border-slate-200 bg-white">
          <table className="min-w-full text-sm">
            <thead className="bg-slate-50 border-b border-slate-200">
              <tr>
                <Th>Employee</Th>
                <Th>Leave Type</Th>
                <Th>Start</Th>
                <Th>End</Th>
                <Th>Days</Th>
                <Th>Status</Th>
              </tr>
            </thead>
            <tbody>
              {leaves.map((l) => (
                <tr key={l.id} className="border-b last:border-b-0 hover:bg-slate-50">
                  <Td>{l.employeeName}</Td>
                  <Td>{l.leaveTypeName}</Td>
                  <Td>{l.startDate ? l.startDate.substring(0, 10) : "-"}</Td>
                  <Td>{l.endDate ? l.endDate.substring(0, 10) : "-"}</Td>
                  <Td>{l.days}</Td>
                  <Td>{l.status}</Td>
                </tr>
              ))}
              {leaves.length === 0 && (
                <tr>
                  <Td colSpan={6} className="text-center text-slate-500">
                    No leave requests found.
                  </Td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

function Th({ children }) {
  return (
    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
      {children}
    </th>
  );
}

function Td({ children, colSpan }) {
  return (
    <td
      className="px-4 py-2 text-sm text-slate-800 align-middle"
      colSpan={colSpan}
    >
      {children}
    </td>
  );
}

export default LeavesPage;


