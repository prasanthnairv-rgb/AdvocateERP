import { useState } from 'react';
import Login from './components/Login';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(!!localStorage.getItem('token'));

  if (!isAuthenticated) {
    return <Login onLoginSuccess={() => setIsAuthenticated(true)} />;
  }

  return (
    <div>
      <h1>Welcome to AdvocateERP Dashboard</h1>
      <button onClick={() => { localStorage.clear(); setIsAuthenticated(false); }}>Logout</button>
      {/* Your tenant-filtered data components will go here */}
    </div>
  );
}

export default App;