import React, { Component } from 'react';
import { Navigate } from 'react-router-dom';
import CommentCard from '../Components/PostCard/CommentCard';
import './PostDetail.css';

class PostDetail extends Component {
  constructor(props) {
    super(props);
    this.state = {
      post: null,
      comments: [],
      newComment: '',
      error: null,
      redirectToHome: false,
    };
  }

  async componentDidMount() {
    const { postId } = this.getPostIdFromUrl();
    await this.fetchPostDetail(postId);
    await this.fetchComments(postId);
  }

  getPostIdFromUrl = () => {
    const pathname = window.location.pathname;
    const postId = pathname.split('/').pop();
    return { postId };
  };

  fetchPostDetail = async (postId) => {
    try {
      const response = await fetch(`http://localhost:5072/api/post/getPost/${postId}`);
      if (!response.ok) throw new Error('Post detayı alınamadı');
      const data = await response.json();
      this.setState({ post: data[0] });
    } catch (error) {
      console.error('Post detayı alınamadı: ', error);
      this.setState({ error: 'Post detayı alınamadı' });
    }
  };

  fetchComments = async (postId) => {
    try {
      const response = await fetch(`http://localhost:5072/api/comment/getComments/${postId}`);
      if (!response.ok) throw new Error('Yorumlar alınamadı');
      const data = await response.json();
      this.setState({ comments: data });
    } catch (error) {
      console.error('Yorumlar alınamadı: ', error);
      this.setState({ error: 'Yorumlar alınamadı' });
    }
  };

  handleCommentChange = (event) => {
    this.setState({ newComment: event.target.value });
  };

  handleCommentSubmit = async (event) => {
    event.preventDefault();
    const { post, newComment } = this.state;
    const userId = localStorage.getItem("UserId");

    try {
      const response = await fetch("http://localhost:5072/api/comment/addComment", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          PostId: post.PostId,
          UserId: userId,
          Content: newComment,
        }),
      });

      if (response.ok) {
        this.setState({ newComment: "" });
        await this.fetchComments(post.PostId);
      }
    } catch (error) {
      console.error("Yorum eklenemedi: ", error);
      this.setState({ error: "Yorum eklenemedi" });
    }
  };

  render() {
    const { post, comments, newComment, error, redirectToHome } = this.state;

    if (redirectToHome) {
      return <Navigate to="/home" />;
    }

    if (error) {
      return <p>{error}</p>;
    }

    return (
      <>
      <div className="card post-detail">
        {post ? (
          <div className="post-card">
            <div className="post-info d-flex justify-content-between">
              <p><strong>Author:</strong> {post.Username}</p>
              <p><strong>Category:</strong> {post.CategoryName}</p>
            </div>
            <h2 className="post-title">{post.Title}</h2>
            <p className="post-content">{post.Content}</p>
          </div>
        ) : (
          <p>Post does not exist anymore</p>
        )}
      </div>
      
      <div className="comments-section">
        <h3 className="comments-title" >Comments</h3>
        
        <form onSubmit={this.handleCommentSubmit}>
          <div className='card comments-card'>
          <textarea className="comments-text"
            value={newComment}
            onChange={this.handleCommentChange}
            placeholder="Yorumunuzu buraya yazın..."
            required
          />
          <button className="comments-button" type="submit">Send</button>
          </div>
        </form>
        
        {comments.map((comment) => (
          <CommentCard
            key={comment.CommentId}
            username={comment.UserName}
            content={comment.Content}
            date={comment.CreateDate}
          />
        ))}
        
      </div>
      </>
    );
  }
}

export default PostDetail;
