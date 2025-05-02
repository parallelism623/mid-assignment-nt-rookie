import { Card } from "antd";
import { environment } from "../../constants/environment";
import { useParams } from "react-router";
import { FiMail, FiUser, FiBook, FiShield, FiPhone } from "react-icons/fi";

import { useUserContext } from "../../routes/ProtectedRoute";

const UserInfoCard = () => {
  const {
    email,
    firstName,
    lastName,
    bookBorrowingLimit,
    roleName,
    phoneNumber,
  } = useUserContext();
  const { id } = useParams();
  return (
    <Card
      title="User Information"
      className="max-w-90 w-fit p-4 rounded-2xl shadow-md"
      headStyle={{ fontWeight: "bold", fontSize: "18px", textAlign: "center" }}
    >
      <div className="flex flex-col gap-4 text-base text-gray-700">
        <div className="flex items-center gap-2">
          <FiMail size={18} className="text-blue-600" />
          <span className="font-semibold">Email:</span> {email}
        </div>
        <div className="flex items-center gap-2">
          <FiUser size={18} className="text-green-600" />
          <span className="font-semibold">Name:</span> {firstName} {lastName}
        </div>
        <div className="flex items-center gap-2">
          <FiPhone size={18} className="text-pink-600" />
          <span className="font-semibold">Phone:</span> {phoneNumber}
        </div>
        {roleName !== environment.adminRole && (
          <div className="flex items-center gap-2">
            <FiBook size={18} className="text-purple-600" />
            <span className="font-semibold">Book Borrow Limit:</span>{" "}
            {bookBorrowingLimit}
          </div>
        )}
        {roleName === environment.adminRole && (
          <div className="flex items-center gap-2">
            <FiShield size={18} className="text-yellow-600" />
            <span className="font-semibold">Role:</span> {roleName}
          </div>
        )}
      </div>
    </Card>
  );
};

export default UserInfoCard;
