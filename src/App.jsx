import "./App.css";
import { useRoutes } from "react-router";
import { appRoutes } from "./routes/AppRoutes";
import AuthProvider, { useAuthContext } from "./components/context/AuthContext";
import NotificationProvider from "./components/context/NotificationContext";
import MessageProvider from "./components/context/MessageContext";
import ForbiddenPage from "./pages/Forbidden/forbidden";
function App() {
  let elements = useRoutes(appRoutes);
  return (
    <>
      {
        <MessageProvider>
          <NotificationProvider>
            <AuthProvider>{elements}</AuthProvider>
          </NotificationProvider>
        </MessageProvider>
      }
    </>
  );
}

export default App;
