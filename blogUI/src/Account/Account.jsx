import React, { useState } from 'react';
import "./Account.css"


class Account extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      users: [],
      username: '',
      email: '',
      password: ''
    };
    this.API_URL = "http://localhost:5072/";
    this.addUser = this.addUser.bind(this);
    this.loginUser = this.loginUser.bind(this);
  }

  //Register
  async addUser() {
    const username = document.getElementById("signUsername").value;
    const email = document.getElementById("signEmail").value;
    const password = document.getElementById("signPassword").value;
 

    let data = {"username":username,"email":email,"password":password,};
    
    try {
      const response = await fetch(`${this.API_URL}api/account/AddUser`, { 
        method: "POST",
        headers: {
          'Accept': 'application/json, text/plain',
          'Content-Type': 'application/json;charset=UTF-8'
      },
        body: JSON.stringify(data)
      });
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      const result = await response.json();
      alert(result);
      this.refreshUsers();
    } catch (error) {
      console.error("Error adding user:", error);
    }
  }

  //Login
  async loginUser() {
    const email = document.getElementById("logEmail").value;
    const password = document.getElementById("logPassword").value;

    let data = { "email": email, "password": password };

    try {
      const response = await fetch(`${this.API_URL}api/account/LoginUser`, { 
        method: "POST",
        headers: {
          'Accept': 'application/json, text/plain',
          'Content-Type': 'application/json;charset=UTF-8'
        },
        body: JSON.stringify(data)
      });
      const result = await response.json();

      if (result.Username) {
        localStorage.setItem("Username", result.Username);
        localStorage.setItem("UserId", result.UserId)
          window.location.href = "/profile";
      }

      else {
        alert('Login failed');
      }
    } catch (error) {
      console.error("Error logging in user:", error);
    }
  }

  render() {
    const { users } = this.state;

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
                      
                      
                      <div className="card-front">
                        <div className="center-wrap">
                          <div className="section text-center">
                            <h4 className="mb-4 pb-3">Log In</h4>
                            <div className="form-group">
                              <input type="email" name="logEmail" className="form-style" placeholder="Email" id="logEmail" autoComplete="off"/>
                              <i className="input-icon uil uil-at"></i>
                            </div>	
                            <div className="form-group mt-2">
                              <input type="password" name="logPass" className="form-style" placeholder="Password" id="logPassword" autoComplete="off"/>
                              <i className="input-icon uil uil-lock-alt"></i>
                            </div>
                            <a href="#" className="btn mt-4" onClick={this.loginUser}>Log In</a>
                              </div>
                            </div>
                          </div>


                      <div className="card-back">
                        <div className="center-wrap">
                          <div className="section text-center">
                            <h4 className="mb-4 pb-3">Sign Up</h4>
                            <div className="form-group">
                              <input type="text" name="signUsername" className="form-style" placeholder="Username" id="signUsername" autoComplete="off"/>
                              <i className="input-icon uil uil-user"></i>
                            </div>	
                            <div className="form-group mt-2">
                              <input type="email" name="signEmail" className="form-style" placeholder="Email" id="signEmail" autoComplete="off"/>
                              <i className="input-icon uil uil-at"></i>
                            </div>	
                            <div className="form-group mt-2">
                              <input type="password" name="signPass" className="form-style" placeholder="Password" id="signPassword" autoComplete="off"/>
                              <i className="input-icon uil uil-lock-alt"></i>
                            </div>
                            <a href="#" className="btn mt-4" onClick={this.addUser}>Sign Up</a>
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
