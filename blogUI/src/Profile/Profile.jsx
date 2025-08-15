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
      username: "",
      email: "",
      createDate: "",
      roles: [],
      userId: "",
      isLoading: true,
      error: null
    };
    this.API_URL = "http://localhost:5072/";
  }

  // Kullanıcı profil bilgilerini backend'den al
  fetchUserProfile = async () => {
    try {
      const response = await fetch(`${this.API_URL}api/auth/profile`, {
        method: 'GET',
        credentials: 'include', // Cookie'leri gönder
        headers: {
          'Content-Type': 'application/json',
        }
      });

      if (!response.ok) {
        if (response.status === 401) {
          // Kullanıcı giriş yapmamış, login sayfasına yönlendir
          if (this.props.history) {
            this.props.history.push('/register');
          } else {
            window.location.href = '/register';
          }
          return;
        }
        throw new Error("Profil bilgileri alınamadı.");
      }

      const userData = await response.json();
      this.setState({
        bio: userData.Bio || "",
        profilePicture: userData.ProfilePicture || "",
        username: userData.Username,
        email: userData.Email,
        createDate: userData.CreateDate,
        roles: userData.Roles || [],
        userId: userData.Id,
        isLoading: false
      });

      // Profil bilgileri alındıktan sonra postları getir
      await this.fetchUserPosts(userData.Id);

    } catch (error) {
      console.error("Profil bilgileri alınamadı:", error);
      this.setState({ 
        error: error.message,
        isLoading: false 
      });
    }
  };

  fetchUserPosts = async (userId) => {
    try {
      const response = await fetch(
        `${this.API_URL}api/post/GetUserPosts?userId=${userId}`,
        {
          credentials: 'include' // Cookie'leri gönder
        }
      );

      if (!response.ok) throw new Error("Postlar alınamadı.");

      const postData = await response.json();
      this.setState({ posts: postData });
    } catch (error) {
      console.error("Postlar alınamadı:", error);
      // Post hatası profil yüklenmesini engellemez
    }
  };

  async componentDidMount() {
    await this.fetchUserProfile();
  }

  // Tarih formatlama helper
  formatDate = (dateString) => {
    if (!dateString) return "";
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  render() {
    const { 
      posts, 
      bio, 
      profilePicture, 
      username, 
      email,
      createDate,
      roles,
      userId,
      isLoading, 
      error 
    } = this.state;

    // Loading durumu
    if (isLoading) {
      return (
        <div className="container">
          <div className="loading-container" style={{ textAlign: 'center', padding: '50px' }}>
            <i className="uil uil-spinner-alt loading-spinner" style={{ fontSize: '48px' }}></i>
            <p>Profil yükleniyor...</p>
          </div>
        </div>
      );
    }

    // Hata durumu
    if (error) {
      return (
        <div className="container">
          <div className="error-container" style={{ textAlign: 'center', padding: '50px' }}>
            <i className="uil uil-exclamation-triangle" style={{ fontSize: '48px', color: '#e74c3c' }}></i>
            <h3>Bir hata oluştu</h3>
            <p>{error}</p>
            <button 
              className="btn btn-primary" 
              onClick={() => window.location.reload()}
            >
              Tekrar Dene
            </button>
          </div>
        </div>
      );
    }

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
                        src={profilePicture || "https://i.pinimg.com/736x/15/0f/a8/150fa8800b0a0d5633abc1d1c4db3d87.jpg"}
                        className="profile-img"
                        alt={username}
                        onError={(e) => {
                          e.target.src = "https://i.pinimg.com/736x/15/0f/a8/150fa8800b0a0d5633abc1d1c4db3d87.jpg";
                        }}
                      />
                    </div>
                    <div className="name">{username}</div>
                  </div>
                  
                  <div className="profile-info">
                    <div className="bio">{bio || "Biyografi bilgisi yok."}</div>
                    <div className="email" style={{ fontSize: '14px', color: '#666', marginTop: '10px' }}>
                      <i className="uil uil-envelope"></i> {email}
                    </div>
                    <div className="join-date" style={{ fontSize: '12px', color: '#999', marginTop: '5px' }}>
                      <i className="uil uil-calendar-alt"></i> Katılım: {this.formatDate(createDate)}
                    </div>
                    {roles.length > 0 && (
                      <div className="roles" style={{ marginTop: '10px' }}>
                        {roles.map((role, index) => (
                          <span 
                            key={index} 
                            className="role-badge"
                            style={{
                              display: 'inline-block',
                              backgroundColor: '#007bff',
                              color: 'white',
                              padding: '2px 8px',
                              borderRadius: '12px',
                              fontSize: '11px',
                              marginRight: '5px'
                            }}
                          >
                            {role}
                          </span>
                        ))}
                      </div>
                    )}
                  </div>

                  {/* Her zaman edit butonu göster çünkü kendi profilini görüyor */}
                  <Link to="/edit-profile">
                    <button className="edit-button">
                      <i className="uil uil-edit"></i> Edit Profile
                    </button>
                  </Link>
                </div>
              </div>
            </div>
          </div>

          <div className="posts">
            <div className="posts-header" style={{ marginBottom: '20px' }}>
              <h3>
                <i className="uil uil-postcard"></i> 
                Paylaşımlar ({posts.length})
              </h3>
            </div>
            
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
              <div className="no-posts" style={{ 
                textAlign: 'center', 
                padding: '40px',
                backgroundColor: '#f8f9fa',
                borderRadius: '10px',
                border: '2px dashed #dee2e6'
              }}>
                <i className="uil uil-document-info" style={{ fontSize: '48px', color: '#6c757d' }}></i>
                <h4 style={{ color: '#6c757d', marginTop: '15px' }}>Henüz post paylaşmamış</h4>
                <p style={{ color: '#6c757d' }}>İlk postunu paylaşmak için yazı yazmaya başla!</p>
                <Link to="/add-post" className="btn btn-primary" style={{ marginTop: '15px' }}>
                  <i className="uil uil-plus"></i> İlk Postunu Yaz
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    );
  }
}

export default Profile;