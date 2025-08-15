import { Navigate } from "react-router-dom";
import { useEffect, useState } from "react";

const ProtectedAdminRoute = ({ children }) => {
  const [loading, setLoading] = useState(true);
  const [isAllowed, setIsAllowed] = useState(false);

  useEffect(() => {
    fetch("http://localhost:5072/api/auth/user", {
      credentials: "include"
    })
      .then(res => res.ok ? res.json() : null)
      .then(userData => {
        if (userData && userData.Roles?.includes("Admin")) {
          setIsAllowed(true);
        }
        setLoading(false);
      })
      .catch(() => setLoading(false));
  }, []);

  if (loading) return <div>Loading...</div>;

  return isAllowed ? children : <Navigate to="/home" replace />;
};

export default ProtectedAdminRoute;
