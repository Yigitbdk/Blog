import React, { Component } from "react";
import { Link } from "react-router-dom";
import PostCard from "../Components/PostCard/PostCard";
import "./Profile.css";

class Profile extends Component {
  constructor(props) {
    super(props);
    this.state = {
      posts: [],
      bio: "",
      profilePicture: "",
      loggedInUsername: localStorage.getItem("Username"),
      loggedInUserId: localStorage.getItem("UserId"),
    };
  }


  fetchUserProfile = async () => {
    const { loggedInUserId } = this.state;

    try {
      const response = await fetch(
        `http://localhost:5072/api/account/GetUserProfile?userId=${loggedInUserId}`
      );

      if (!response.ok) throw new Error("Profil bilgileri alınamadı.");

      const data = await response.json();
      this.setState({
        bio: data.Bio,
        profilePicture: data.ProfilePicture || "",
      });
    } catch (error) {
      console.error("Profil bilgileri alınamadı:", error);
    }
  };

  fetchUserPosts = async () => {
    const { loggedInUserId } = this.state;

    try {
      const response = await fetch(
        `http://localhost:5072/api/post/GetUserPosts?userId=${loggedInUserId}`
      );

      if (!response.ok) throw new Error("Postlar alınamadı.");

      const postData = await response.json();
      this.setState({ posts: postData });
    } catch (error) {
      console.error("Postlar alınamadı:", error);
    }
  };

  async componentDidMount() {
    await this.fetchUserProfile();
    await this.fetchUserPosts();
  }

  render() {
    const { posts, bio, profilePicture, loggedInUsername, loggedInUserId } = this.state;
    const currentUserId = localStorage.getItem("UserId");

    return (
      <div className="container">
        <div className="flex-row">
          <div className="card profil-card">
            <div className="card-body">
              <div className="d-flex">
                <div className="col-4 justify-content-start">
                  <div className="flex-row gap-20">
                    <div>
                      <img
                        src={profilePicture || "https://media.licdn.com/dms/image/v2/D4D03AQGW6eizbhLWHA/profile-displayphoto-shrink_400_400/profile-displayphoto-shrink_400_400/0/1683136188553?e=2147483647&v=beta&t=e6LQSWqNe1znaiR7tNrGCvBqU_P6p8p1DtPYQNyCr7A"}
                        className="profile-img"
                        alt={loggedInUsername}
                      />
                    </div>
                    <div className="name">{loggedInUsername}</div>
                  </div>
                  <div className="bio">{bio || "Biyografi bilgisi yok."}</div>

                  {currentUserId === loggedInUserId && (
                    <Link to="/edit-profile">
                      <button className="edit-button">Edit</button>
                    </Link>
                  )}
                </div>
              </div>
            </div>
          </div>

          <div className="posts">
            {posts.length > 0 ? (
              posts.map((post) => (
                <PostCard
                  key={post.PostId}
                  post={post}
                  category={post.CategoryName}
                  username={post.Username}
                />
              ))
            ) : (
              <p>Henüz post paylaşmamış.</p>
            )}
          </div>
        </div>
      </div>
    );
  }
}

export default Profile;
