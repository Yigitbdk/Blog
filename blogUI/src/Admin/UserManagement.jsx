import React, { useState, useEffect } from 'react';
import { Users, Shield, ShieldCheck, Search, ChevronLeft, ChevronRight, AlertCircle, CheckCircle, UserCheck } from 'lucide-react';

const UserManagement = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [notification, setNotification] = useState(null);
  const usersPerPage = 10;

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const response = await fetch('http://localhost:5072/api/auth/admin/users', {
        method: 'GET',
        credentials: 'include',
        headers: { 'Content-Type': 'application/json' }
      });

      if (response.ok) {
        const usersData = await response.json();
        setUsers(usersData);
      } else {
        showNotification('Kullanıcı listesi alınamadı', 'error');
      }
    } catch (error) {
      console.error('Fetch users error:', error);
      showNotification('Sunucu hatası', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showNotification = (message, type = 'success') => {
    setNotification({ message, type });
    setTimeout(() => setNotification(null), 3000);
  };

  const toggleAdminRole = async (userId) => {
    setLoading(true);
    try {
      const user = users.find(u => u.Id === userId);
      const hasAdminRole = user.Roles.includes('Admin');
      const action = hasAdminRole ? 'remove' : 'add';

      const response = await fetch(`http://localhost:5072/api/auth/admin/users/${userId}/roles`, {
        method: 'PUT',
        credentials: 'include',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ action, role: 'Admin' })
      });

      if (response.ok) {
        setUsers(prevUsers => 
          prevUsers.map(u => {
            if (u.Id === userId) {
              const newRoles = hasAdminRole
                ? u.Roles.filter(r => r !== 'Admin')
                : [...u.Roles, 'Admin'];
              return { ...u, Roles: newRoles };
            }
            return u;
          })
        );

        showNotification(
          hasAdminRole
            ? `${user.UserName} kullanıcısından Admin rolü kaldırıldı`
            : `${user.UserName} kullanıcısına Admin rolü verildi`,
          'success'
        );
      } else {
        const error = await response.json();
        showNotification(error.message || 'İşlem başarısız', 'error');
      }
    } catch (error) {
      console.error('Toggle admin role error:', error);
      showNotification('Sunucu hatası oluştu', 'error');
    } finally {
      setLoading(false);
    }
  };

  const filteredUsers = users.filter(user =>
    (user.UserName || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
    (user.Email || '').toLowerCase().includes(searchTerm.toLowerCase())
  );

  const indexOfLastUser = currentPage * usersPerPage;
  const indexOfFirstUser = indexOfLastUser - usersPerPage;
  const currentUsers = filteredUsers.slice(indexOfFirstUser, indexOfLastUser);
  const totalPages = Math.ceil(filteredUsers.length / usersPerPage);

  const nextPage = () => {
    if (currentPage < totalPages) {
      setCurrentPage(currentPage + 1);
    }
  };

  const prevPage = () => {
    if (currentPage > 1) {
      setCurrentPage(currentPage - 1);
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-white rounded-lg shadow-sm border p-6">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-2xl font-bold text-slate-900 flex items-center space-x-3">
              <Users className="h-7 w-7 text-blue-600" />
              <span>Kullanıcı Rol Yönetimi</span>
            </h2>
            <p className="text-slate-600 mt-2">Kullanıcılara admin rolü atayın veya kaldırın</p>
          </div>
          <div className="flex items-center space-x-2 bg-slate-50 px-4 py-2 rounded-lg">
            <UserCheck className="h-5 w-5 text-slate-600" />
            <span className="text-sm font-medium text-slate-700">{filteredUsers.length} kullanıcı</span>
          </div>
        </div>
      </div>

      {/* Notification */}
      {notification && (
        <div className={`p-4 rounded-lg flex items-center space-x-3 shadow-sm ${
          notification.type === 'success' 
            ? 'bg-green-50 text-green-800 border border-green-200' 
            : 'bg-red-50 text-red-800 border border-red-200'
        }`}>
          {notification.type === 'success' ? 
            <CheckCircle className="h-5 w-5 text-green-600" /> : 
            <AlertCircle className="h-5 w-5 text-red-600" />
          }
          <span className="font-medium">{notification.message}</span>
        </div>
      )}

      {/* Search */}
      <div className="bg-white rounded-lg shadow-sm border p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-slate-400" />
          <input
            type="text"
            placeholder="Kullanıcı adı veya e-posta ile ara..."
            className="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
            value={searchTerm}
            onChange={(e) => { 
              setSearchTerm(e.target.value); 
              setCurrentPage(1); 
            }}
          />
        </div>
      </div>

      {/* Users Table */}
      <div className="bg-white rounded-lg shadow-sm border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead className="bg-slate-50 border-b border-slate-200">
              <tr>
                <th className="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase tracking-wider">
                  Kullanıcı
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase tracking-wider">
                  E-posta
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase tracking-wider">
                  Roller
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase tracking-wider">
                  Durum
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase tracking-wider">
                  İşlemler
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-200">
              {currentUsers.map(user => (
                <tr key={user.Id} className="hover:bg-slate-50 transition-colors">
                  <td className="px-6 py-4">
                    <div className="flex items-center space-x-4">
                      <div className="h-10 w-10 rounded-full bg-gradient-to-br from-blue-400 to-blue-600 flex items-center justify-center shadow-sm">
                        {user.ProfilePicture ? (
                          <img 
                            src={user.ProfilePicture} 
                            alt={user.UserName} 
                            className="h-10 w-10 rounded-full object-cover" 
                          />
                        ) : (
                          <span className="text-sm font-bold text-white">
                            {(user.UserName || user.Email)?.charAt(0).toUpperCase()}
                          </span>
                        )}
                      </div>
                      <div>
                        <div className="text-sm font-semibold text-slate-900">
                          {user.UserName || user.Email}
                        </div>
                        {user.UserName && user.UserName !== user.Email && (
                          <div className="text-xs text-slate-500">{user.Email}</div>
                        )}
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4 text-sm text-slate-700">{user.Email}</td>
                  <td className="px-6 py-4">
                    <div className="flex flex-wrap gap-2">
                      {user.Roles.map(role => (
                        <span 
                          key={role} 
                          className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-medium ${
                            role === 'Admin' 
                              ? 'bg-red-100 text-red-700 border border-red-200' 
                              : 'bg-slate-100 text-slate-700 border border-slate-200'
                          }`}
                        >
                          {role === 'Admin' && <ShieldCheck className="w-3 h-3 mr-1" />}
                          {role}
                        </span>
                      ))}
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-medium ${
                      user.IsActive 
                        ? 'bg-green-100 text-green-700 border border-green-200' 
                        : 'bg-slate-100 text-slate-700 border border-slate-200'
                    }`}>
                      {user.IsActive ? 'Aktif' : 'Pasif'}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <button
                      onClick={() => toggleAdminRole(user.Id)}
                      disabled={loading}
                      className={`inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg transition-all duration-200 ${
                        user.Roles.includes('Admin') 
                          ? 'text-red-700 bg-red-100 hover:bg-red-200 focus:ring-red-500 border-red-200 hover:border-red-300' 
                          : 'text-blue-700 bg-blue-100 hover:bg-blue-200 focus:ring-blue-500 border-blue-200 hover:border-blue-300'
                      } focus:outline-none focus:ring-2 focus:ring-offset-2 ${
                        loading ? 'opacity-50 cursor-not-allowed' : 'shadow-sm hover:shadow'
                      }`}
                    >
                      {loading ? (
                        <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-current mr-2"></div>
                      ) : (
                        <Shield className="h-4 w-4 mr-2" />
                      )}
                      {user.Roles.includes('Admin') ? 'Admin Kaldır' : 'Admin Yap'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="bg-slate-50 px-6 py-3 border-t border-slate-200">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-2 text-sm text-slate-600">
                <span>
                  Sayfa {currentPage} / {totalPages} 
                </span>
                <span className="text-slate-400">•</span>
                <span>
                  Toplam {filteredUsers.length} kullanıcı
                </span>
              </div>
              <div className="flex items-center space-x-2">
                <button
                  onClick={prevPage}
                  disabled={currentPage === 1}
                  className="p-2 rounded-lg border border-slate-300 text-slate-600 hover:bg-slate-100 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                >
                  <ChevronLeft className="h-4 w-4" />
                </button>
                <button
                  onClick={nextPage}
                  disabled={currentPage === totalPages}
                  className="p-2 rounded-lg border border-slate-300 text-slate-600 hover:bg-slate-100 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                >
                  <ChevronRight className="h-4 w-4" />
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default UserManagement;