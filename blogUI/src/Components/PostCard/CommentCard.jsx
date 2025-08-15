import React from 'react';
import './CommentCard.css';

const CommentCard = ({ username, content, date }) => {
  return (
    <div className="card comment-card">
      <div className="comment-info">
        <p className="username">{username}</p>
        <p className="date">{new Date(date).toLocaleString()}</p>
      </div>
      <p className="comment-content">{content}</p>
    </div>
  );
};

export default CommentCard;