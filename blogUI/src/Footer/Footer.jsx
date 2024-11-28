import React from 'react';
import './Footer.css';

function Footer () {
  return (
  <>
        <footer id="footer" >

        <div className="col col1">

            <h1>bLog.</h1>
            <p>Made with <span> <i className="uil uil-react"/> </span> by Me</p>

            <div className="social">
            <a href="https://www.linkedin.com/in/yi%C4%9Fit-badik-953457206/" target="_blank" className="link"><i className="uil uil-linkedin"/></a>
            <a href="https://twitter.com" target="_blank" className="link"><i className="uil uil-twitter"/></a>
            <a href="https://youtube.com" target="_blank" className="link"><i className="uil uil-youtube"/></a>
            </div>

            <p> 2024 © All Rights Reserved </p>

        </div>

        <div className="col col2">
            <p>About</p>
            <p>Our mission</p>
            <p>Privacy Policy</p>
            <p>Terms of service</p>
        </div>

        <div className="col col3">
            <p>Services</p>
            <p>Products</p>
            <p>Join our team</p>
            <p>Partner with us</p>
        </div>
        <div className="backdrop"></div>
        </footer>
</>
  );
}

export default Footer;