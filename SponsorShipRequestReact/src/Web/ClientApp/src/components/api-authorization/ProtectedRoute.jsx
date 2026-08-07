import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from './AuthContext';

export function ProtectedRoute({ children, requiredRole }) {
    const { isAuthenticated, isLoading, userRoles } = useAuth();
  const location = useLocation();

    if (isLoading) return null;

    if (!isAuthenticated) {
        return (
            <Navigate
                to="/login"
                state={{ returnUrl: location.pathname }}
                replace
            />
        );
    }

    if (requiredRole && !userRoles?.includes(requiredRole)) {
        console.log("ProtectedRoute: required role NOT found");
        return <Navigate to="/unauthorized" replace />; //todo implementaion
    }
    console.log("ProtectedRoute: rendering children");

  return children;
}
