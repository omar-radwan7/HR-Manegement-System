import { BrowserRouter, Routes, Route, Navigate, Link } from "react-router-dom";
import "./index.css";
import EmployeesPage from "./pages/EmployeesPage.jsx";
import EmployeeDetailsPage from "./pages/EmployeeDetailsPage.jsx";
import DashboardPage from "./pages/DashboardPage.jsx";
import LeavesPage from "./pages/LeavesPage.jsx";

function Layout({ children }) {
  return (
    <div className="min-h-screen flex bg-slate-100">
      <aside className="w-64 bg-slate-900 text-slate-100 flex flex-col">
        <div className="px-6 py-4 text-xl font-semibold border-b border-slate-800">
          HRM System
        </div>
        <nav className="flex-1 px-4 py-4 space-y-2 text-sm">
          <NavItem to="/dashboard" label="Dashboard" />
          <NavItem to="/employees" label="Employees" />
          <NavItem to="/leaves" label="Leaves" />
        </nav>
      </aside>
      <main className="flex-1 flex flex-col">
        <header className="h-14 bg-white border-b border-slate-200 flex items-center justify-between px-6">
          <h1 className="text-lg font-semibold text-slate-800">HR Management</h1>
          <div className="text-xs text-slate-500">
            Branch: <span className="font-medium">Head Office</span>
          </div>
        </header>
        <section className="flex-1 p-6">{children}</section>
      </main>
    </div>
  );
}

function NavItem({ to, label }) {
  return (
    <Link
      to={to}
      className="block px-3 py-2 rounded-md hover:bg-slate-800 hover:text-white transition-colors"
    >
      {label}
    </Link>
  );
}

function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/employees" element={<EmployeesPage />} />
          <Route path="/employees/:id" element={<EmployeeDetailsPage />} />
          <Route path="/leaves" element={<LeavesPage />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}

export default App;
