import { BrowserRouter as Router, Routes, Route, Navigate, useLocation } from "react-router-dom";
import Navbarr from "./Navbar/Navbar.jsx";
import Footer from "./Footer/Footer.jsx";
import Account from "./Account/Account.jsx";
import Profile from "./Profile/Profile.jsx";
import PostList from "./Post/PostList.jsx";
import AddPost from "./Post/AddPost.jsx";
import EditProfile from "./Profile/EditProfile.jsx";
import PostDetail from "./Post/PostDetail.jsx";
import AdminPanel from "./Admin/AdminPanel.jsx";
import ProtectedAdminRoute from "./Components/ProtectedAdminRoute.jsx";

function AppContent() {
  const location = useLocation();

  // Admin routes için navbar ve footer gizle
  const isAdminRoute = location.pathname.startsWith("/admin");

  return (
    <div className="flex min-h-screen">
      {/* Ana site navbar'ı - sadece admin dışındaki sayfalarda göster */}
      {!isAdminRoute && <Navbarr />}

      {/* Ana içerik alanı */}
      <div className={`flex-grow ${!isAdminRoute ? '' : ''}`}>
        <Routes>
          {/* Ana site rotaları */}
          <Route path="/register" element={<Account />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="/edit-profile" element={<EditProfile />} />
          <Route path="/home" element={<PostList />} />
          <Route path="/add-post" element={<AddPost />} />
          <Route path="/post/:postId" element={<PostDetail />} />

          {/* Admin panel rotası - kendi navbar'ı var */}
          <Route
            path="/admin/*"
            element={
              <ProtectedAdminRoute>
                <AdminPanel />
              </ProtectedAdminRoute>
            }
          />

          {/* Varsayılan yönlendirme */}
          <Route path="/" element={<Navigate to="/home" replace />} />
          <Route path="*" element={<Navigate to="/home" replace />} />
        </Routes>
      </div>

      {/* Ana site footer'ı - sadece admin dışındaki sayfalarda göster */}
      {!isAdminRoute && <Footer />}
    </div>
  );
}

function App() {
  return (
    <Router>
      <AppContent />
    </Router>
  );
}

export default App;