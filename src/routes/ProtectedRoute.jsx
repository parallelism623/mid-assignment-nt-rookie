import { useNavigate } from "react-router";
import React, { useContext, useEffect, useState } from "react";
import LoadingPage from "../pages/LoadingPage/LoadingPage";
import { useLocalStorage } from "../components/hooks/useStorage";
import { jwtServices } from "../services/jwtServices";
import { userServices } from "../services/userServices";
import { useMessageContext } from "../components/context/MessageContext";

const UserContext = React.createContext({
  roleName: "",
  roleId: "",
  bookBorrowingLimit: null,
  id: "",
  firstName: "",
  lastName: "",
  useName: "",
  email: "",
});

export const useUserContext = () => {
  return useContext(UserContext);
};
const ProtectedRoutes = (props) => {
  const { message } = useMessageContext();
  const navigate = useNavigate();
  const { children } = props;
  const [accessToken, setAccessToken, removeAccessToken] = useLocalStorage(
    "access_token",
    ""
  );
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const tokenDecode = jwtServices.decode(accessToken);
    const sid = tokenDecode?.sid;
    if (!sid) {
      localStorage.removeItem("access_token");
      return navigate("/signin");
    }
    userServices
      .getById(sid)
      .then((res) => {
        setUser(res);
        setLoading(false);
      })
      .catch(() => {
        localStorage.removeItem("access_token");
        return navigate("/signin");
      });
  }, [accessToken]);

  return (
    <>
      <UserContext.Provider value={{ ...user }}>
        {loading && <LoadingPage />}
        {!loading && user && children}
      </UserContext.Provider>
    </>
  );
};

export default ProtectedRoutes;
