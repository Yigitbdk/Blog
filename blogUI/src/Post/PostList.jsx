import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import PostCard from '../Components/PostCard/PostCard';
import './PostList.css';

class PostList extends Component {
  constructor(props) {
    super(props);
    this.state = {
      categories: [],
      posts: [],
      filteredPosts: [],
      selectedCategory: null,
      isLoggedIn: false,
    };
  }

  async componentDidMount() {
    this.checkLoginStatus();
    await this.fetchCategories();
    await this.fetchPosts();
  }

  checkLoginStatus = () => {
    const user = localStorage.getItem('Username');
    this.setState({ isLoggedIn: !!user });
  };

  fetchCategories = async () => {
    try {
      const response = await fetch('http://localhost:5072/api/category/getCategories');
      if (!response.ok) throw new Error('Kategoriler alınamadı');
      const data = await response.json();
      this.setState({ categories: data });
    } catch (error) {
      console.error('Kategoriler alınamadı: ', error);
    }
  };

  fetchPosts = async () => {
    try {
      const response = await fetch('http://localhost:5072/api/post/getPosts');
      if (!response.ok) throw new Error('Postlar alınamadı');
      const data = await response.json();
      this.setState({ posts: data, filteredPosts: data });
    } catch (error) {
      console.error('Postlar alınamadı: ', error);
    }
  };

  handleCategoryClick = async (categoryId) => {
    this.setState({ selectedCategory: categoryId });
    if (categoryId) {
      try {
        const response = await fetch(`http://localhost:5072/api/post/CategoryFilter?categoryId=${categoryId}`);
        if (!response.ok) throw new Error('Postlar alınamadı');
        const data = await response.json();
        this.setState({ filteredPosts: data });
      } catch (error) {
        console.error('Postlar alınamadı: ', error);
      }
    } else {
      this.setState({ filteredPosts: this.state.posts });
    }
  };

  render() {
    const { categories, filteredPosts, isLoggedIn } = this.state;

    return (
      <div className="post-list">
        <div className="categories">
          <ul>
            <li>
              <button type="button" onClick={() => this.handleCategoryClick(null)}>
                All
              </button>
            </li>
            {categories.map((category) => (
              <li key={category.CategoryId}>
                <button
                  type="button"
                  onClick={() => this.handleCategoryClick(category.CategoryId)}
                >
                  {category.Name}
                </button>
              </li>
            ))}
          </ul>
        </div>

        <div className="posts">
          {isLoggedIn && (
            <div>
              <Link to="/add-post"> <button type="button" className="add-post-button">
              What do you think today..?
              </button> </Link>
            </div>
          )}
          {filteredPosts.length > 0 ? (
            filteredPosts.map((filteredPost) => (
              <Link to={`/post/${filteredPost.PostId}`} key={filteredPost.PostId}>
                <PostCard
                  post={filteredPost}
                  category={filteredPost.CategoryName}
                  username={filteredPost.Username}
                />
              </Link>
            ))
          ) : (
            <h44 className='xxx'>Bu kategoride post bulunamadı.</h44>
          )}
        </div>
      </div>
    );
  }
}

export default PostList;
