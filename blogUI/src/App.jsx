import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Navbarr from "./navbar/Navbar.jsx";
import Footer from "./Footer/Footer.jsx";
import Account from "./Account/Account.jsx";
import Profile from "./Profile/Profile.jsx";
import PostList from "./Post/PostList.jsx";
import AddPost from "./Post/AddPost.jsx";
import EditProfile from "./Profile/EditProfile.jsx";
import PostDetail from "./Post/PostDetail.jsx";
import { Navigate } from "react-router-dom";

function App() {
  return (
    <Router>
      <Navbarr/>

      <Routes>
        <Route path="/register" element={<Account />} />
        <Route path="/profile" element={<Profile />} />
        <Route path="/edit-profile" element={<EditProfile />} />
        <Route path="/home" element={<PostList />} />
        <Route path="/add-post" element={<AddPost />} />
        <Route path="/post/:postId" element={<PostDetail />} />
        <Route path="*" index element={<Navigate to="/home" replace/>}/>
      </Routes>

      <Footer/>
    </Router>
  );
}

export default App;
