import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';

class Navbarr extends Component {
  constructor(props) {
    super(props);
    this.state = {
      username: '',
      isLoggedIn: false,
      isAdmin: false, // Admin kontrolü için eklendi
      isLoading: true
    };
    this.API_URL = "http://localhost:5072/";
  }

  componentDidMount() {
    this.checkAuthStatus();
  }

  // Kullanıcının giriş durumunu kontrol et
  checkAuthStatus = async () => {
    try {
      const response = await fetch(`${this.API_URL}api/auth/user`, { // Endpoint'i güncelledik
        method: 'GET',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json',
        }
      });

      if (response.ok) {
        const userData = await response.json();
        this.setState({
          username: userData.Username || userData.UserName || 'User',
          isLoggedIn: true,
          isAdmin: userData.Roles && userData.Roles.includes('Admin'), // Admin kontrolü
          isLoading: false
        });
      } else {
        this.setState({
          username: '',
          isLoggedIn: false,
          isAdmin: false,
          isLoading: false
        });
      }
    } catch (error) {
      console.error('Auth check failed:', error);
      this.setState({
        username: '',
        isLoggedIn: false,
        isAdmin: false,
        isLoading: false
      });
    }
  };

  handleLogout = async () => {
    try {
      const response = await fetch(`${this.API_URL}api/auth/logout`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include'
      });

      if (response.ok) {
        const result = await response.json();
        console.log('Logout successful:', result.Message);
      } else {
        console.error('Logout failed');
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      this.setState({ 
        username: '', 
        isLoggedIn: false,
        isAdmin: false // Admin durumunu da temizle
      });
      
      if (this.props.history) {
        this.props.history.push('/home');
      } else {
        window.location.href = '/home';
      }
    }
  };

  render() {
    const { username, isLoggedIn, isAdmin, isLoading } = this.state;

    // Loading durumu
    if (isLoading) {
      return (
        <nav>
          <div className="wrapper">
            <div className="logo">
              <a href="#"><span>bLog.</span></a>
            </div>
            <ul className="nav-links">
              <li><Link to="/home">Home</Link></li>
              <li><Link to="/about">About</Link></li>
              <li className="log-in">
                <i className="uil uil-spinner-alt loading-spinner"></i>
                <span className='padding-5'>Loading...</span>
              </li>
            </ul>
          </div>
        </nav>
      );
    }

    return (
      <nav>
        <div className="wrapper">
          <div className="logo">
            <a href="#"><span>bLog.</span></a>
          </div>

          <ul className="nav-links">
            <li><Link to="/home">Home</Link></li>
            <li><Link to="/about">About</Link></li>
            
            {/* Admin Panel Linki - Sadece admin kullanıcılar için */}
            {isAdmin && (
              <li className="admin-panel">
                <Link 
                  to="/admin" 
                  style={{ 
                    color: '#dc2626', 
                    fontWeight: 'bold',
                    textDecoration: 'none'
                  }}
                  title="Admin Panel"
                >
                  Admin Panel
                </Link>
              </li>
            )}

            <li className={isLoggedIn ? "user-profile" : "log-in"}>
              {!isLoggedIn && <i className="uil uil-user-square fs-24"></i>}
              <Link className='padding-5' to={isLoggedIn ? "/profile" : "/register"}>
                {isLoggedIn ? username : 'Log In'}
              </Link>
              {isLoggedIn && (
                <i 
                  className="logout-btn uil uil-sign-out-alt" 
                  onClick={this.handleLogout}
                  title="Logout"
                  style={{ cursor: 'pointer' }}
                ></i>
              )}
            </li>
          </ul>
        </div>
      </nav>
    );
  }
}

export default Navbarr;