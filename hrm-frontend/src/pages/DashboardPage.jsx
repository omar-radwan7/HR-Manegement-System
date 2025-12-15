function DashboardPage() {
  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold text-slate-800">Dashboard</h2>
      <p className="text-sm text-slate-600">
        High level overview of your HR metrics (employees, leaves, attendance, and approvals).
      </p>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
          <div className="text-xs uppercase text-slate-500">Employees</div>
          <div className="mt-2 text-2xl font-semibold text-slate-800">-</div>
        </div>
        <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
          <div className="text-xs uppercase text-slate-500">Open Leave Requests</div>
          <div className="mt-2 text-2xl font-semibold text-slate-800">-</div>
        </div>
        <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
          <div className="text-xs uppercase text-slate-500">Today&apos;s Attendance</div>
          <div className="mt-2 text-2xl font-semibold text-slate-800">-</div>
        </div>
      </div>
    </div>
  );
}

export default DashboardPage;


