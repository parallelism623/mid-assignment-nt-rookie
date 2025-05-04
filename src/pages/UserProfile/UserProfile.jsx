import { Row, Col, List, Card } from "antd";
import UserInfoCard from "./UserInfoCard";
import {
  HistoryOutlined,
  SettingOutlined,
  UserOutlined,
} from "@ant-design/icons";
import ActivityLogPanel from "./ActivityLogPanel";
import UserUpdateProfile from "./UserUpdateProfile";
import { useState } from "react";
import ChangePassword from "./ChangePassword";

const menuItems = [
  {
    title: "Activity Log",
    icon: <HistoryOutlined />,
  },
  {
    title: "Settings",
    icon: <SettingOutlined />,
  },
  {
    title: "Profile",
    icon: <UserOutlined />,
  },
];

const UserProfile = () => {
  const [option, setOption] = useState(0);
  const handleOnOptionChange = (op) => {
    setOption(op);
    console.log(op);
  };
  return (
    <div className="p-6">
      <Row gutter={[16, 16]}>
        <Col xs={24} md={6}>
          <UserInfoCard
            currentOption={option}
            handleOnClickOptions={(op) => {
              handleOnOptionChange(op);
            }}
          />
        </Col>
        <Col xs={24} md={18}>
          {option === 0 && <ActivityLogPanel />}
          {option === 1 && <UserUpdateProfile />}
          {option === 3 && <ChangePassword />}
        </Col>
      </Row>
    </div>
  );
};

export default UserProfile;
