import AuthProvider from './AuthProvider';
import AppRouter from "./router/AppRouter";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";


function App() {
  return (
    <div className="App">
     <Router>
        <AuthProvider>
          <AppRouter />
        </AuthProvider>
      </Router>
    </div>
  );
}

export default App;
