import React, { Children } from "react";
import { Button, message } from "antd";
const MessageContext = React.createContext({ message: null });
export const useMessageContext = () => {
  return React.useContext(MessageContext);
};
const MessageProvider = (props) => {
  const { children } = props;
  const [messageApi, contextHolder] = message.useMessage();

  return (
    <>
      {contextHolder}
      <MessageContext.Provider value={{ message: messageApi }}>
        {children}
      </MessageContext.Provider>
    </>
  );
};
export default MessageProvider;
