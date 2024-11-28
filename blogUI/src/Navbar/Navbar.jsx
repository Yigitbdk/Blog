import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';

class Navbarr extends Component {
  constructor(props) {
    super(props);
    this.state = {
      username: '',
    };
  }

  componentDidMount() {
    const loggedInUser = localStorage.getItem("Username");
    if (loggedInUser) {
      this.setState({ username: loggedInUser });
    }
  }

  handleLogout = () => {
    localStorage.removeItem("Username");
    this.setState({ username: null });
    this.props.history.push('/home');
    window.location.reload();
  };

  render() {
    const { username } = this.state;

    return (
      <nav>
        <div className="wrapper">
          <div className="logo">
            <a href="#"><span>bLog.</span></a>
          </div>

          <ul className="nav-links">
            <li><Link to="/home">Home</Link></li>
            <li><Link to="/about">About</Link></li>

            <li className={username ? "user-profile" : "log-in"}>
              {!username && <i className="uil uil-user-square fs-24"></i>}
              <Link className='padding-5' to={username ? "/profile" : "/register"}>
                {username ? username : 'Log In'}
              </Link>
              {username && <div className='logout-btn' onClick={this.handleLogout}>Logout</div>}
            </li>
          </ul>
        </div>
      </nav>
    );
  }
}

export default Navbarr;
