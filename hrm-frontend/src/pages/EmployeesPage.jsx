import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

const API_BASE = "http://localhost:5000"; // HRM.Api default
const DEFAULT_BRANCH_ID = ""; // TODO: set to a valid Branch GUID from your seed data

function EmployeesPage() {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);
        setError(null);
        const res = await fetch(`${API_BASE}/api/employees`, {
          headers: {
            "X-Branch-Id": DEFAULT_BRANCH_ID,
          },
        });
        if (!res.ok) {
          throw new Error(`API error: ${res.status}`);
        }
        const data = await res.json();
        setEmployees(data);
      } catch (err) {
        setError(err.message || "Failed to load employees");
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []);

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-800">Employees</h2>
        <button className="px-3 py-2 text-sm font-medium rounded-md bg-slate-900 text-white hover:bg-slate-800">
          New Employee
        </button>
      </div>

      {loading && <p className="text-sm text-slate-500">Loading employees...</p>}
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
                <Th>Employee #</Th>
                <Th>Name</Th>
                <Th>Email</Th>
                <Th>Department</Th>
                <Th>Position</Th>
                <Th>Status</Th>
              </tr>
            </thead>
            <tbody>
              {employees.map((e) => (
                <tr key={e.id} className="border-b last:border-b-0 hover:bg-slate-50">
                  <Td>
                    <Link
                      to={`/employees/${e.id}`}
                      className="text-slate-900 font-medium hover:underline"
                    >
                      {e.employeeNumber}
                    </Link>
                  </Td>
                  <Td>{e.firstName} {e.lastName}</Td>
                  <Td>{e.email}</Td>
                  <Td>{e.departmentName}</Td>
                  <Td>{e.positionTitle}</Td>
                  <Td>
                    <span
                      className={
                        "inline-flex items-center rounded-full px-2 py-0.5 text-xs font-semibold " +
                        (e.isActive
                          ? "bg-emerald-50 text-emerald-700"
                          : "bg-slate-100 text-slate-500")
                      }
                    >
                      {e.isActive ? "Active" : "Inactive"}
                    </span>
                  </Td>
                </tr>
              ))}
              {employees.length === 0 && (
                <tr>
                  <Td colSpan={6} className="text-center text-slate-500">
                    No employees found.
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

export default EmployeesPage;


