import { Link, useNavigate } from 'react-router-dom';
import { useAuth, SystemRoles } from './api-authorization/AuthContext';
import { ThemeToggle } from './ThemeToggle';

function AuthLinks() {
    const { isAuthenticated, logout, userRoles, userEmail } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async (e) => {
    e.preventDefault();
    await logout();
    navigate('/login');
  };

  if (isAuthenticated) {
      return (
          <>
              <li>
                  Hello {userEmail} |{" "}
                  {userRoles &&
                      userRoles.map((item, i) => (
                          <span key={i}>{item}</span>
                      ))}
              </li>
              <li><a href="#" onClick={handleLogout}>Log out</a></li>
          </>
      );
  }
  return (
    <>
      <li><Link to="/login">Log in</Link></li>
      <li><Link to="/register">Register</Link></li>
    </>
  );
}

export function NavMenu() {

  const {  userRoles } = useAuth();

  return (
    <header>
      <nav>
        <ul>
          <li><Link to="/">Clean Architecture</Link></li>
        </ul>
        <ul>
            <li><Link to="/">Home</Link></li>
            <li><Link to="/counter">Counter</Link></li>
            <li><Link to="/weather">Weather</Link></li>
            <li><Link to="/sponsorship">Sponsorship</Link></li>
            { userRoles?.includes(SystemRoles.Manager) && (
                <li><Link to="/sponsorship-manager-approval">Manager Approval</Link></li>
            )}
            {userRoles?.includes(SystemRoles.FinanceAdmin) && (
                <li><Link to="/sponsorship-finance-approval">Finance Approval</Link></li>
            )}
        </ul>
        <ul>
          <AuthLinks />
          <li aria-hidden="true" className="nav-separator"></li>
          <li><ThemeToggle /></li>
        </ul>
      </nav>
    </header>
  );
}
