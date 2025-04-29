import { notification } from "antd";
import { createContext, useContext } from "react";
import { setNotifier } from "../../config/axiosConfig";
const NotificationContext = createContext({ notification: null });
export const useNotificationContext = () => {
  return useContext(NotificationContext);
};
const NotificationProvider = ({ children }) => {
  const [api, contextHolder] = notification.useNotification();
  setNotifier(api);
  return (
    <>
      {contextHolder}
      <NotificationContext.Provider value={{ api }}>
        {children}
      </NotificationContext.Provider>
    </>
  );
};

export default NotificationProvider;
