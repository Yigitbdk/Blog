import React from 'react';
import './AddPost.css';

class AddPost extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      title: '',
      content: '',
      categoryId: null,
      categories: []
    };
  }

  // Kategorileri backend'den al
  fetchCategories = async () => {
    try {
      const response = await fetch('http://localhost:5072/api/category/getCategories');
      if (!response.ok) throw new Error('Kategoriler alınamadı');
      const data = await response.json();
      this.setState({ categories: data });
    } catch (error) {
      console.error('Kategoriler alınamadı:', error);
    }
  };

  componentDidMount() {
    this.fetchCategories();
  }

  // Post gönderimi
  handleSubmit = async (e) => {
    e.preventDefault();

    const userId = localStorage.getItem("UserId");
    if (!userId) {
      console.error("Kullanıcı ID'si bulunamadı");
      return;
    }

    const post = {
      title: this.state.title,
      content: this.state.content,
      userId,
      categoryId: this.state.categoryId ? parseInt(this.state.categoryId, 10) : null
    };

    try {
      const response = await fetch('http://localhost:5072/api/post/AddPost', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(post)
      });

      if (!response.ok) throw new Error('Post eklenemedi');
      const result = await response.json();
      console.log('Post başarıyla eklendi:', result);

      window.location.href = '/home';
    } catch (error) {
      console.error('Post eklenemedi:', error);
    }
  };

  render() {
    const { title, content, categoryId, categories } = this.state;

    return (
      <div className="card add-post-container">
        <form onSubmit={this.handleSubmit}>
          <div className="form-group1">
            <input className='text-title'
              placeholder='Title...'
              type="text"
              id="title"
              value={title}
              
              onChange={(e) => this.setState({ title: e.target.value })}
              required
            />
          </div>

          <div className="form-group1">
            <textarea className="text-content"
              placeholder='Content...'
              id="content"
              value={content}
              onChange={(e) => this.setState({ content: e.target.value })}
              required
            ></textarea>
          </div>

          <div className="form-group1 row-bot">
            <select
              className="cat-post"
              id="category"
              value={categoryId || ''}
              onChange={(e) => this.setState({ categoryId: e.target.value })}
            >
              <option value="">Kategori seçin</option>
              {categories.map((category) => (
                <option key={category.CategoryId} value={category.CategoryId}>
                  {category.Name}
                </option>
              ))}
            </select>

            <button
              className="button-post"
              type="submit">Post
            </button>
          </div>
        </form>
      </div>
    );
  }
}

export default AddPost;
