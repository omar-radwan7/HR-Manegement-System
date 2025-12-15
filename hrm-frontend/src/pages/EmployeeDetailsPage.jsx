import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const API_BASE = "http://localhost:5000";
const DEFAULT_BRANCH_ID = "";

function EmployeeDetailsPage() {
  const { id } = useParams();
  const [employee, setEmployee] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);
        setError(null);
        const res = await fetch(`${API_BASE}/api/employees/${id}`, {
          headers: {
            "X-Branch-Id": DEFAULT_BRANCH_ID,
          },
        });
        if (!res.ok) {
          throw new Error(`API error: ${res.status}`);
        }
        const data = await res.json();
        setEmployee(data);
      } catch (err) {
        setError(err.message || "Failed to load employee");
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [id]);

  if (loading) {
    return <p className="text-sm text-slate-500">Loading employee...</p>;
  }

  if (error) {
    return <p className="text-sm text-red-600">{error}</p>;
  }

  if (!employee) {
    return <p className="text-sm text-slate-500">Employee not found.</p>;
  }

  return (
    <div className="space-y-4 max-w-3xl">
      <h2 className="text-2xl font-semibold text-slate-800">
        {employee.firstName} {employee.lastName}
      </h2>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 bg-white border border-slate-200 rounded-lg p-4">
        <Info label="Employee #" value={employee.employeeNumber} />
        <Info label="Email" value={employee.email} />
        <Info label="Phone" value={employee.phone || "-"} />
        <Info label="Department" value={employee.departmentName} />
        <Info label="Position" value={employee.positionTitle} />
        <Info label="Branch" value={employee.branchName} />
        <Info
          label="Hire Date"
          value={employee.hireDate ? employee.hireDate.substring(0, 10) : "-"}
        />
        <Info label="Status" value={employee.isActive ? "Active" : "Inactive"} />
      </div>
    </div>
  );
}

function Info({ label, value }) {
  return (
    <div>
      <div className="text-xs uppercase text-slate-500">{label}</div>
      <div className="mt-1 text-sm text-slate-800">{value}</div>
    </div>
  );
}

export default EmployeeDetailsPage;


