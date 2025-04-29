import { FaUserCircle } from "react-icons/fa";
import { Popover, Menu, Avatar } from "antd";
import { Link, useNavigate } from "react-router";
import "../../assets/styles/AccountMenuStyle.css";
import {
  UserOutlined,
  SettingOutlined,
  LogoutOutlined,
} from "@ant-design/icons";
import { authenServices } from "../../services/authenServices";
import { useLocalStorage } from "../hooks/useStorage";

const content = (onClickLogout) => (
  <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
    <Link to="users/profile">
      <UserOutlined style={{ paddingRight: "0.6em" }} />
      Profile
    </Link>
    <Link to="/settings">
      <SettingOutlined style={{ paddingRight: "0.6em" }} />
      Settings
    </Link>
    <Link onClick={onClickLogout} to="/logout">
      <LogoutOutlined style={{ paddingRight: "0.6em" }} />
      Logout
    </Link>
  </div>
);
const AccountMenu = () => {
  const navigate = useNavigate();
  const [accessToken, setAccessToken, removeAccessToken] = useLocalStorage(
    "access_token",
    ""
  );
  const onClickLogout = () => {
    authenServices.logout().then(() => {
      localStorage.removeItem("access_token");
      localStorage.removeItem("refresh_token");
      navigate("/signin");
    });
  };
  const accountMenuItem = content(onClickLogout);
  return (
    <>
      <Popover
        overlayClassName="popover-account-menu"
        getPopupContainer={() => document.body}
        placement="bottomRight"
        content={accountMenuItem}
      >
        <span className="icon-wrapper">
          <FaUserCircle />
        </span>
      </Popover>
    </>
  );
};

export default AccountMenu;
