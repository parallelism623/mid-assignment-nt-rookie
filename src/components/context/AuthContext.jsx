import { useContext, useState, createContext} from "react";
import { environment } from "../../constants/environment";



const AuthContext = createContext({
    isAuthenticated: false,
    setIsAuthenticated: () => { },
});
export const useAuthContext = () => {
    useContext(AuthContext);
}

const AuthProvider = ({ children }) => {
    const token = localStorage.getItem(environment.accessToken);
    const [isAuthenticated, setIsAuthenticated] = useState(!!token);

    const contextValue = {
        isAuthenticated,
        setIsAuthenticated,
    }
    return (
        <AuthContext.Provider value={contextValue}>
            {children}
        </AuthContext.Provider>
    )
}


export default AuthProvider;