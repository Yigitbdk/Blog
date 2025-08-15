import React from 'react';
import './EditProfile.css';

class EditProfile extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      username: '',
      bio: '',
      profilePicture: '',
      email: '',
      isLoading: true,
      isSaving: false,
      error: null,
      successMessage: '',
      errors: {
        bio: '',
        profilePicture: '',
        general: ''
      }
    };
    this.API_URL = "http://localhost:5072/";
  }

  componentDidMount() {
    this.loadUserProfile();
  }

  // Kullanıcı profil bilgilerini backend'den al
  loadUserProfile = async () => {
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
        throw new Error('Profil verileri alınamadı');
      }

      const data = await response.json();
      this.setState({
        username: data.Username || '',
        bio: data.Bio || '',
        profilePicture: data.ProfilePicture || '',
        email: data.Email || '',
        isLoading: false
      });
    } catch (error) {
      console.error('Profil verileri alınamadı:', error);
      this.setState({ 
        error: error.message,
        isLoading: false 
      });
    }
  };

  // Clear all errors
  clearErrors = () => {
    this.setState({
      errors: {
        bio: '',
        profilePicture: '',
        general: ''
      },
      successMessage: ''
    });
  };

  // Set specific error
  setError = (field, message) => {
    this.setState(prevState => ({
      errors: {
        ...prevState.errors,
        [field]: message
      }
    }));
  };

  // Validation
  validateForm = () => {
    const { bio, profilePicture } = this.state;
    let hasError = false;

    this.clearErrors();

    // Bio validation
    if (bio.length > 1000) {
      this.setError('bio', 'Bio cannot exceed 1000 characters');
      hasError = true;
    }

    // Profile picture URL validation (basic)
    if (profilePicture && !this.isValidUrl(profilePicture)) {
      this.setError('profilePicture', 'Please enter a valid URL');
      hasError = true;
    }

    return !hasError;
  };

  // URL validation helper
  isValidUrl = (string) => {
    try {
      new URL(string);
      return true;
    } catch (_) {
      return false;
    }
  };

  handleSave = async (e) => {
    e.preventDefault();

    if (!this.validateForm()) {
      return;
    }

    const updatedProfile = {
       Bio: this.state.bio || ""
      };

    if (this.state.profilePicture) {
      updatedProfile.ProfilePicture = this.state.profilePicture;
    }



    try {
      this.setState({ isSaving: true });
      this.clearErrors();

      const response = await fetch(`${this.API_URL}api/auth/profile`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include', // Cookie'leri gönder
        body: JSON.stringify(updatedProfile)
      });

      const result = await response.json();

      if (response.ok) {
        // Success
        this.setState({ successMessage: result.Message });
        console.log('Profil başarıyla güncellendi:', result);
        
        // 2 saniye sonra profile sayfasına yönlendir
        setTimeout(() => {
          if (this.props.history) {
            this.props.history.push('/profile');
          } else {
            window.location.href = '/profile';
          }
        }, 2000);

      } else {
        // Handle backend validation errors
        if (result.errors) {
          // Handle validation errors from ASP.NET Core
          for (const [field, messages] of Object.entries(result.errors)) {
            const errorMessages = Array.isArray(messages) ? messages : [messages];
            
            switch(field.toLowerCase()) {
              case 'bio':
                this.setError('bio', errorMessages[0]);
                break;
              case 'profilepicture':
                this.setError('profilePicture', errorMessages[0]);
                break;
              default:
                this.setError('general', `${field}: ${errorMessages[0]}`);
                break;
            }
          }
        } else if (result.Errors && Array.isArray(result.Errors)) {
          this.setError('general', result.Errors.join(', '));
        } else if (typeof result === 'string') {
          this.setError('general', result);
        } else if (result.message) {
          this.setError('general', result.message);
        } else {
          this.setError('general', 'Profile update failed. Please try again.');
        }
      }

    } catch (error) {
      console.error('Profil güncellenemedi:', error);
      this.setError('general', 'Network error. Please check your connection and try again.');
    } finally {
      this.setState({ isSaving: false });
    }
  };

  render() {
    const { 
      username, 
      bio, 
      profilePicture, 
      email,
      isLoading, 
      isSaving, 
      error, 
      successMessage,
      errors 
    } = this.state;

    // Loading durumu
    if (isLoading) {
      return (
        <div className="edit-profile-container">
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
        <div className="edit-profile-container">
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
      <div className="edit-profile-container">
        <div className="card">
          <div className="card-header">
            <h2>
              <i className="uil uil-edit"></i> 
              Profili Düzenle
            </h2>
          </div>

          <form onSubmit={this.handleSave}>
            
            {/* Username (Read-only) */}
            <div className="form-group">
              <label htmlFor="username">
                <i className="uil uil-user"></i> Kullanıcı Adı
              </label>
              <input
                type="text"
                id="username"
                placeholder="Kullanıcı Adı"
                value={username}
                disabled
                style={{ backgroundColor: '#f8f9fa', cursor: 'not-allowed' }}
              />
              <small style={{ color: '#6c757d' }}>Kullanıcı adı değiştirilemez</small>
            </div>

            {/* Email (Read-only) */}
            <div className="form-group">
              <label htmlFor="email">
                <i className="uil uil-envelope"></i> Email
              </label>
              <input
                type="email"
                id="email"
                placeholder="Email"
                value={email}
                disabled
                style={{ backgroundColor: '#f8f9fa', cursor: 'not-allowed' }}
              />
              <small style={{ color: '#6c757d' }}>Email değiştirilemez</small>
            </div>

            {/* Bio */}
            <div className={`form-group ${errors.bio ? 'error' : ''}`}>
              <label htmlFor="bio">
                <i className="uil uil-edit-alt"></i> Biyografi
                <span style={{ color: bio.length > 950 ? '#e74c3c' : bio.length > 900 ? '#f39c12' : '#6c757d' }}>
                  ({bio.length}/1000)
                </span>
              </label>
              <textarea
                id="bio"
                placeholder="Kendiniz hakkında birkaç kelime..."
                value={bio}
                onChange={(e) => {
                  this.setState({ bio: e.target.value });
                  if (errors.bio) this.setError('bio', '');
                }}
                maxLength="1000"
                rows="4"
              />
              {errors.bio && <div className="error-message">{errors.bio}</div>}
            </div>

            {/* Profile Picture */}
            <div className={`form-group ${errors.profilePicture ? 'error' : ''}`}>
              <label htmlFor="profilePicture">
                <i className="uil uil-image"></i> Profil Resmi URL'si
              </label>
              <input
                type="text"
                id="profilePicture"
                placeholder="https://example.com/profile-image.jpg"
                value={profilePicture}
                onChange={(e) => {
                  this.setState({ profilePicture: e.target.value });
                  if (errors.profilePicture) this.setError('profilePicture', '');
                }}
              />
              {errors.profilePicture && <div className="error-message">{errors.profilePicture}</div>}
              
              {/* Image Preview */}
              {profilePicture && this.isValidUrl(profilePicture) && (
                <div className="image-preview" style={{ marginTop: '10px' }}>
                  <img 
                    src={profilePicture} 
                    alt="Preview" 
                    style={{ 
                      width: '80px', 
                      height: '80px', 
                      borderRadius: '50%', 
                      objectFit: 'cover',
                      border: '2px solid #dee2e6'
                    }}
                    onError={(e) => {
                      e.target.style.display = 'none';
                    }}
                  />
                  <small style={{ display: 'block', color: '#6c757d', marginTop: '5px' }}>
                    Önizleme
                  </small>
                </div>
              )}
            </div>

            {/* Success Message */}
            {successMessage && (
              <div className="success-message" style={{
                display: 'block',
                padding: '10px',
                backgroundColor: '#d4edda',
                border: '1px solid #c3e6cb',
                borderRadius: '5px',
                color: '#155724',
                marginBottom: '15px'
              }}>
                <i className="uil uil-check"></i> {successMessage}
              </div>
            )}

            {/* General Error */}
            {errors.general && (
              <div className="error-message" style={{
                display: 'block',
                color: '#e74c3c',
                fontSize: '14px',
                marginBottom: '15px',
                padding: '10px',
                backgroundColor: '#f8d7da',
                border: '1px solid #f5c6cb',
                borderRadius: '5px'
              }}>
                <i className="uil uil-exclamation-triangle"></i> {errors.general}
              </div>
            )}

            {/* Action Buttons */}
            <div className="form-actions">
              <button 
                type="button" 
                className="btn btn-secondary"
                onClick={() => {
                  if (this.props.history) {
                    this.props.history.push('/profile');
                  } else {
                    window.location.href = '/profile';
                  }
                }}
                disabled={isSaving}
              >
                <i className="uil uil-times"></i> İptal
              </button>
              
              <button 
                type="submit" 
                className="btn btn-primary"
                disabled={isSaving}
              >
                {isSaving ? (
                  <>
                    <i className="uil uil-spinner-alt loading-spinner"></i> 
                    Güncelleniyor...
                  </>
                ) : (
                  <>
                    <i className="uil uil-check"></i> 
                    Profili Güncelle
                  </>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>
    );
  }
}

export default EditProfile;