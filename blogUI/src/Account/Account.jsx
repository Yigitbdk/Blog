import React, { useState } from 'react';
import "./Account.css"

class Account extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      users: [],
      username: '',
      email: '',
      password: '',
      // Register form validation states
      registerErrors: {
        username: '',
        email: '',
        password: '',
        bio: '',
        general: ''
      },
      successMessage: '',
      isRegistering: false,
      bioCharCount: 0
    };
    this.API_URL = "http://localhost:5072/";
    this.addUser = this.addUser.bind(this);
    this.loginUser = this.loginUser.bind(this);
    this.handleBioChange = this.handleBioChange.bind(this);
    this.clearRegisterErrors = this.clearRegisterErrors.bind(this);
    this.validateField = this.validateField.bind(this);
  }

  // Clear all registration errors
  clearRegisterErrors = () => {
    this.setState({
      registerErrors: {
        username: '',
        email: '',
        password: '',
        bio: '',
        general: ''
      },
      successMessage: ''
    });
  }

  // Set specific error
  setRegisterError = (field, message) => {
    this.setState(prevState => ({
      registerErrors: {
        ...prevState.registerErrors,
        [field]: message
      }
    }));
  }

  // Set success message
  setSuccessMessage = (message) => {
    this.setState({ successMessage: message });
  }

  // Email validation helper
  isValidEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  // Real-time field validation
  validateField = (field, value) => {
    let error = '';
    
    switch(field) {
      case 'username':
        if (value && (value.length < 3 || value.length > 50)) {
          error = value.length < 3 ? 'Username must be at least 3 characters' : 'Username cannot exceed 50 characters';
        }
        break;
      case 'email':
        if (value && !this.isValidEmail(value)) {
          error = 'Please enter a valid email address';
        }
        break;
      case 'password':
        if (value && (value.length < 6 || value.length > 100)) {
          error = value.length < 6 ? 'Password must be at least 6 characters' : 'Password cannot exceed 100 characters';
        }
        break;
      case 'bio':
        if (value.length > 500) {
          error = 'Bio cannot exceed 500 characters';
        }
        break;
    }
    
    this.setRegisterError(field, error);
  }

  // Handle bio character counting
  handleBioChange = (e) => {
    const value = e.target.value;
    this.setState({ bioCharCount: value.length });
    this.validateField('bio', value);
  }

  // Register function
  addUser = async () => {
    this.clearRegisterErrors();
    
    const username = document.getElementById('signUsername').value.trim();
    const email = document.getElementById('signEmail').value.trim();
    const password = document.getElementById('signPassword').value;
    const bio = document.getElementById('signBio').value.trim();
    
    // Frontend validation
    let hasError = false;
    
    if (!username) {
      this.setRegisterError('username', 'Username is required');
      hasError = true;
    } else if (username.length < 3 || username.length > 50) {
      this.setRegisterError('username', username.length < 3 ? 'Username must be at least 3 characters' : 'Username cannot exceed 50 characters');
      hasError = true;
    }
    
    if (!email) {
      this.setRegisterError('email', 'Email is required');
      hasError = true;
    } else if (!this.isValidEmail(email)) {
      this.setRegisterError('email', 'Please enter a valid email');
      hasError = true;
    }
    
    if (!password) {
      this.setRegisterError('password', 'Password is required');
      hasError = true;
    } else if (password.length < 6 || password.length > 100) {
      this.setRegisterError('password', password.length < 6 ? 'Password must be at least 6 characters' : 'Password cannot exceed 100 characters');
      hasError = true;
    }
    
    if (bio.length > 500) {
      this.setRegisterError('bio', 'Bio cannot exceed 500 characters');
      hasError = true;
    }
    
    if (hasError) return;
    
    const registerData = {
      Username: username,
      Email: email,
      Password: password,
      Bio: bio || ""
    };
    
    try {
      this.setState({ isRegistering: true });
      
      const response = await fetch(`${this.API_URL}api/auth/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(registerData)
      });
      
      const result = await response.json();
      
      if (response.ok) {
        // Success handling
        this.setSuccessMessage(`Registration successful! Welcome ${result.Username}!`);
        // Clear form
        document.getElementById('signUsername').value = '';
        document.getElementById('signEmail').value = '';
        document.getElementById('signPassword').value = '';
        document.getElementById('signBio').value = '';
        this.setState({ bioCharCount: 0 });
        console.log('Registration successful:', result);
      } else {
        // Handle backend validation errors
        if (result.errors) {
          // Handle validation errors from ASP.NET Core
          for (const [field, messages] of Object.entries(result.errors)) {
            const errorMessages = Array.isArray(messages) ? messages : [messages];
            
            // Map backend field names to frontend error fields
            switch(field.toLowerCase()) {
              case 'username':
                this.setRegisterError('username', errorMessages[0]);
                break;
              case 'email':
                this.setRegisterError('email', errorMessages[0]);
                break;
              case 'password':
                this.setRegisterError('password', errorMessages[0]);
                break;
              case 'bio':
                this.setRegisterError('bio', errorMessages[0]);
                break;
              default:
                this.setRegisterError('general', `${field}: ${errorMessages[0]}`);
                break;
            }
          }
        } else if (typeof result === 'string') {
          this.setRegisterError('general', result);
        } else if (result.Errors && Array.isArray(result.Errors)) {
          this.setRegisterError('general', result.Errors.join(', '));
        } else if (result.message) {
          this.setRegisterError('general', result.message);
        } else if (result.title) {
          this.setRegisterError('general', result.title);
        } else {
          this.setRegisterError('general', 'Registration failed. Please try again.');
        }
        console.error('Registration failed:', result);
      }
    } catch (error) {
      this.setRegisterError('general', 'Network error. Please check your connection and try again.');
      console.error('Network error:', error);
    } finally {
      this.setState({ isRegistering: false });
    }
  }

  // Login function (unchanged)
  async loginUser() {
    const emailOrUsername = document.getElementById("logEmailOrUsername").value;
    const password = document.getElementById("logPassword").value;
    const rememberMe = document.getElementById("logRememberMe")?.checked || false;

    let data = { EmailOrUsername: emailOrUsername, Password: password, RememberMe: rememberMe };

    try {
      const response = await fetch(`${this.API_URL}api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
        credentials: "include"  // cookie gönder
      });

      if (!response.ok) {
        const errorText = await response.text();
        alert(`Login failed: ${errorText}`);
        return;
      }

      // Login başarılı, kullanıcı bilgisi localStorage'a kaydetme
      // İstersen kullanıcıyı almak için profile endpoint çağırabilirsin

      window.location.href = "/profile";

    } catch (error) {
      alert("Login failed: Network or parsing error");
    }
  }

  render() {
    const { registerErrors, successMessage, isRegistering, bioCharCount } = this.state;

    return (
      <div className="section">
        <div>
          <div className="row full-height justify-content-center">
            <div className="col-12 text-center align-self-center py-5 ">
              <div className="display: block; section pb-5 pt-5 pt-sm-2 text-center">
                <h6 className="mb-0 pb-3"><span>Log In </span> <span>Sign Up</span></h6>
                <input style={{display:'none'}} className="checkbox" type="checkbox" id="reg-log" name="reg-log"/>
                <label htmlFor="reg-log"></label>

                <div className="card-3d-wrap mx-auto">
                  <div className="card-3d-wrapper">
                    
                    {/* Login Form */}
                    <div className="card-front">
                      <div className="center-wrap">
                        <div className="section text-center">
                          <h4 className="mb-4 pb-3">Log In</h4>
                          <div className="form-group">
                            <input
                              type="email"
                              name="logEmailOrUsername"
                              className="form-style"
                              placeholder="Email"
                              id="logEmailOrUsername"
                              autoComplete="off"
                            />
                            <i className="input-icon uil uil-at"></i>
                          </div>
                          <div className="form-group mt-2">
                            <input
                              type="password"
                              name="logPassword"
                              className="form-style"
                              placeholder="Password"
                              id="logPassword"
                              autoComplete="off"
                            />
                            <i className="input-icon uil uil-lock-alt"></i>
                          </div>
                          
                          <div className="form-group mt-3 text-start">
                            <input
                              type="checkbox"
                              id="logRememberMe"
                              name="logRememberMe"
                              className="form-check-input"
                            />
                            <label htmlFor="logRememberMe" className="ms-2">
                              Remember Me
                            </label>
                          </div>
                          
                          <a href="#" className="btn mt-4" onClick={this.loginUser}>
                            Log In
                          </a>
                        </div>
                      </div>
                    </div>

                    {/* Register Form */}
                    <div className="card-back">
                      <div className="center-wrap">
                        <div className="section text-center">
                          <h4 className="mb-4 pb-3">Sign Up</h4>
                          
                          {/* Email */}
                          <div className={`form-group mt ${registerErrors.email ? 'error' : ''}`}>
                            <input 
                              type="email" 
                              name="signEmail" 
                              className="form-style" 
                              placeholder="example@hotmail.com" 
                              id="signEmail" 
                              autoComplete="off"
                              onBlur={(e) => this.validateField('email', e.target.value.trim())}
                              onChange={() => registerErrors.email && this.setRegisterError('email', '')}
                            />
                            <i className="input-icon uil uil-at"></i>
                            {registerErrors.email && <div className="error-message">{registerErrors.email}</div>}
                          </div>

                          {/* Username */}
                          <div className={`form-group mt-2 ${registerErrors.username ? 'error' : ''}`}>
                            <input 
                              type="text" 
                              name="signUsername" 
                              className="form-style" 
                              placeholder="Username" 
                              id="signUsername" 
                              autoComplete="off"
                              onBlur={(e) => this.validateField('username', e.target.value.trim())}
                              onChange={() => registerErrors.username && this.setRegisterError('username', '')}
                            />
                            <i className="input-icon uil uil-user"></i>
                            {registerErrors.username && <div className="error-message">{registerErrors.username}</div>}
                          </div>
                          
                          {/* Password */}
                          <div className={`form-group mt-2 ${registerErrors.password ? 'error' : ''}`}>
                            <input 
                              type="password" 
                              name="signPass" 
                              className="form-style" 
                              placeholder="Password" 
                              id="signPassword" 
                              autoComplete="off"
                              onBlur={(e) => this.validateField('password', e.target.value)}
                              onChange={() => registerErrors.password && this.setRegisterError('password', '')}
                            />
                            <i className="input-icon uil uil-lock-alt"></i>
                            {registerErrors.password && <div className="error-message">{registerErrors.password}</div>}
                          </div>
                          
                          {/* Bio */}
                          <div className={`form-group mt-2 ${registerErrors.bio ? 'error' : ''}`}>
                            <textarea 
                              name="signBio" 
                              className="bio-textarea" 
                              placeholder="Bio (optional - max 500 characters)" 
                              id="signBio" 
                              autoComplete="off"
                              maxLength="500"
                              onChange={this.handleBioChange}
                            />
                            <i className="input-icon uil uil-edit-alt"></i>
                            {registerErrors.bio && <div className="error-message">{registerErrors.bio}</div>}
                            <div style={{fontSize: '11px', color: bioCharCount > 450 ? '#e74c3c' : bioCharCount > 400 ? '#f39c12' : '#666', marginTop: '5px', textAlign: 'right'}}>
                              {bioCharCount}/500
                            </div>
                          </div>
                          
                          {/* Success Message */}
                          {successMessage && (
                            <div className="success-message" style={{display: 'block', padding: '10px', backgroundColor: '#d4edda', border: '1px solid #c3e6cb', borderRadius: '5px', color: '#155724', marginTop: '10px'}}>
                              {successMessage}
                            </div>
                          )}
                          
                          {/* General Error */}
                          {registerErrors.general && (
                            <div className="error-message" style={{display: 'block', color: '#e74c3c', fontSize: '12px', marginTop: '10px'}}>
                              {registerErrors.general}
                            </div>
                          )}
                          
                          <a 
                            href="#" 
                            className="btn mt-4" 
                            onClick={this.addUser}
                            style={{opacity: isRegistering ? 0.6 : 1, pointerEvents: isRegistering ? 'none' : 'auto'}}
                          >
                            {isRegistering ? 'Registering...' : 'Sign Up'}
                          </a>
                        </div>
                      </div>
                    </div>

                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default Account;