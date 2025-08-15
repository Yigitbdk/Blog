import React from 'react';
import './PostCard.css';

function PostCard({ post }) {

  return (
    <div className="flex-col gap-20">
      <div className="row">
        <div className="col-sm">
          <div className="card">
            <div className="card-body">
              <h5 className="card-title">{post.Title}</h5>
              <p className="card-text">{post.Content}</p>
            </div>

            <div className="card-footer d-flex justify-content-between">
              <small className="card-username">{post.User?.Username || 'Anonim'}</small>
              <small className="card-category">{post.Category?.Name || 'Kategori Yok'}</small>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default PostCard;
