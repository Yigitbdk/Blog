import React from 'react';
import './EditProfile.css';

class EditProfile extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      username: '',
      bio: '',
      profilePicture: ''
    };
  }

  componentDidMount() {

    const userId = localStorage.getItem("UserId");
    if (!userId) {
      console.error("Kullanıcı ID'si bulunamadı");
      return;
    }
    

    this.loadUserProfile(userId);
  }

  loadUserProfile = async (userId) => {
    try {
      const response = await fetch(`http://localhost:5072/api/account/GetUserProfile?userId=${userId}`);
      if (!response.ok) throw new Error('Profil verileri alınamadı');
      const data = await response.json();
      this.setState({
        username: data.Username,
        bio: data.Bio,
        profilePicture: data.ProfilePicture || ''
      });
    } catch (error) {
      console.error('Profil verileri alınamadı:', error);
    }
  };

  handleSave = async (e) => {
    e.preventDefault();

    const userId = localStorage.getItem("UserId");
    if (!userId) {
      console.error("Kullanıcı ID'si bulunamadı");
      return;
    }

    const updatedProfile = {
      UserId: userId,
      Username: this.state.username,
      Bio: this.state.bio,
      ProfilePicture: this.state.profilePicture
    };

    try {
      const response = await fetch('http://localhost:5072/api/account/UpdateUserProfile', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(updatedProfile)
      });

      if (!response.ok) throw new Error('Profil güncellenemedi');
      const result = await response.json();
      console.log('Profil başarıyla güncellendi:', result);


      window.location.href = '/profile';
    } catch (error) {
      console.error('Profil güncellenemedi:', error);
    }
  };

  render() {
    const { username, bio, profilePicture } = this.state;

    return (
      <div className="card edit-profile-container">
        <form onSubmit={this.handleSave}>
          <div className="form-group">
            <input
              type="text"
              placeholder="Kullanıcı Adı..."
              value={username}
              onChange={(e) => this.setState({ username: e.target.value })}
              required
            />
          </div>

          <div className="form-group">
            <textarea
              placeholder="Biyografi..."
              value={bio}
              onChange={(e) => this.setState({ bio: e.target.value })}
            />
          </div>

          <div className="form-group">
            <input
              type="text"
              placeholder="Profil Resmi URL'si..."
              value={profilePicture}
              onChange={(e) => this.setState({ profilePicture: e.target.value })}
            />
          </div>

          <button type="submit">Profili Güncelle</button>
        </form>
      </div>
    );
  }
}

export default EditProfile;
