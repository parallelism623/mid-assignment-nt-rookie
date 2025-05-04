import { Card, Divider, List } from "antd";
import {
  FiMail,
  FiUser,
  FiPhone,
  FiBook,
  FiShield,
  FiSettings,
  FiClock,
} from "react-icons/fi";
import { RiLockPasswordLine } from "react-icons/ri";
import { environment } from "../../constants/environment";
import { useUserContext } from "../../routes/ProtectedRoute";
import ActivityLogPanel from "./ActivityLogPanel";
const UserInfoCard = ({ handleOnClickOptions, currentOption }) => {
  const {
    email,
    firstName,
    lastName,
    phoneNumber,
    roleName,
    bookBorrowingLimit,
  } = useUserContext();

  const userInfoItems = [
    {
      label: "Email",
      value: email,
      icon: <FiMail size={18} className="text-blue-600" />,
    },
    {
      label: "Name",
      value: `${firstName} ${lastName}`,
      icon: <FiUser size={18} className="text-green-600" />,
    },
    {
      label: "Phone",
      value: phoneNumber,
      icon: <FiPhone size={18} className="text-pink-600" />,
    },
    roleName !== environment.adminRole
      ? {
          label: "Book Borrow Limit",
          value: bookBorrowingLimit,
          icon: <FiBook size={18} className="text-purple-600" />,
        }
      : {
          label: "Role",
          value: roleName,
          icon: <FiShield size={18} className="text-yellow-600" />,
        },
  ];

  const menuItems = [
    {
      label: "Activity Log",
      icon: <FiClock size={18} />,
      value: 0,
    },
    {
      label: "Update Profile",
      value: 1,
      icon: <FiUser size={18} />,
    },
    {
      label: "Change Password",
      icon: <RiLockPasswordLine size={18} />,
      value: 3,
    },
  ];

  return (
    <>
      <Card
        title="User Information"
        className="w-full max-w-xs rounded-2xl shadow-md"
        headStyle={{
          fontWeight: "bold",
          fontSize: "18px",
          textAlign: "center",
        }}
      >
        <div className="flex flex-col gap-4 text-base text-gray-700">
          {userInfoItems.map((item) => (
            <div
              key={item.label}
              className="flex flex-wrap items-center gap-2 break-all"
            >
              {item.icon}
              <span className="font-semibold">{item.label}:</span> {item.value}
            </div>
          ))}
        </div>

        <Divider className="my-4" />
        <List
          itemLayout="horizontal"
          dataSource={menuItems}
          renderItem={(item) => (
            <List.Item
              onClick={() => handleOnClickOptions(item.value)}
              className="cursor-pointer transition px-1 py-1.5"
            >
              <List.Item.Meta
                className={
                  currentOption == item.value
                    ? "bg-blue-100 !text-blue-400 shadow-sm p-2 rounded-lg w-full"
                    : "p-2"
                }
                avatar={
                  <div className="text-base text-gray-600">{item.icon}</div>
                }
                title={
                  <span
                    className={
                      "text-sm font-medium text-gray-800 transition " +
                      (currentOption == item.value
                        ? "hover:text-[#E6F7FF]"
                        : "hover:text-[#57C4DC]")
                    }
                  >
                    {item.label}
                  </span>
                }
              />
            </List.Item>
          )}
        />
      </Card>
    </>
  );
};

export default UserInfoCard;
