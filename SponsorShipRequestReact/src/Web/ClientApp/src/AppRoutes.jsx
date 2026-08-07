import { Counter } from "./components/Counter";
import { Weather } from "./components/Weather";
// import { Tasks } from "./components/Todo";
import { Home } from "./components/Home";
import { LoginPage } from "./components/api-authorization/LoginPage";
import { RegisterPage } from "./components/api-authorization/RegisterPage";
import { ProtectedRoute } from "./components/api-authorization/ProtectedRoute";
import { Sponsorship } from './components/sponsorships/Sponsorship';
import { SponsorShipApprovalCommon } from './components/sponsorships/SponsorShipApprovalCommon';
import { SystemRoles } from './components/api-authorization/AuthContext';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/weather',
    element: <ProtectedRoute><Weather /></ProtectedRoute>
  },
  // {
  //   path: '/todo',
  //   element: <ProtectedRoute><Tasks /></ProtectedRoute>
    // },
  {
    path: '/sponsorship',
    element: <ProtectedRoute><Sponsorship /></ProtectedRoute>
  },
  {
      path: '/sponsorship-manager-approval',
      element: <ProtectedRoute requiredRole={SystemRoles.Manager} ><SponsorShipApprovalCommon approvalCategory={SystemRoles.Manager} /></ProtectedRoute>
  },
  {
      path: '/sponsorship-finance-approval',
      element: <ProtectedRoute requiredRole={SystemRoles.FinanceAdmin} ><SponsorShipApprovalCommon approvalCategory={SystemRoles.FinanceAdmin} /></ProtectedRoute>
  },
  {
    path: '/login',
    element: <LoginPage />
  },
  {
    path: '/register',
    element: <RegisterPage />
  }
];

export default AppRoutes;
