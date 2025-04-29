import React from "react";
import { Modal, Tag, Tooltip } from "antd";
import {
  FiBook,
  FiUser,
  FiTag,
  FiUserCheck,
  FiCalendar,
  FiRepeat,
  FiEdit3,
  FiCheck,
  FiX,
  FiPlusCircle,
} from "react-icons/fi";
import dayjs from "dayjs";
import { environment } from "../../constants/environment";
import { validationData } from "../../constants/validationData";

const BookBorrowedDetail = ({
  visible,
  onCancel,
  data,
  onApprove,
  onReject,
  roleName,
  userId,
}) => {
  if (!data) return null;

  const {
    book: {
      title,
      author,
      category: { name: categoryName },
    },
    requesterName,
    approverName,
    dueDate,
    extendDueDate,
    extendDueDateTimes,
    noted,
  } = data;

  return (
    <Modal
      visible={visible}
      onCancel={onCancel}
      footer={
        <div className="flex justify-end gap-3">
          {roleName === environment.adminRole && !!data.extendDueDate && (
            <Tooltip title="Approve">
              <button
                onClick={() => onApprove(data.id)}
                className="p-2 rounded-lg bg-green-300 text-green-800 hover:bg-green-400"
              >
                <FiCheck size={18} />
              </button>
            </Tooltip>
          )}
          {roleName === environment.adminRole && !!data.extendDueDate && (
            <Tooltip title="Reject">
              <button
                onClick={() => onReject(data.id)}
                className="p-2 rounded-lg bg-red-300 text-red-800 hover:bg-red-400"
              >
                <FiX size={18} />
              </button>
            </Tooltip>
          )}
          {data.extendDueDateTimes < validationData.bookBorrowedExtendDueDate &&
            roleName !== environment.adminRole && (
              <Tooltip title="Extend Due Date">
                <button
                  onClick={() => onExtend(data.id)}
                  className="p-2 rounded-lg bg-blue-300 text-blue-800 hover:bg-blue-400"
                >
                  <FiPlusCircle size={18} />
                </button>
              </Tooltip>
            )}
        </div>
      }
      maskClassName="bg-black bg-opacity-40"
      className="!p-0"
      bodyStyle={{ padding: 0 }}
    >
      <div className="bg-white rounded-2xl overflow-hidden">
        <div className="px-6 py-4">
          <h2 className="text-2xl font-bold text-center">
            Borrowed Book Detail
          </h2>
        </div>

        <div className="p-6">
          <div className="grid grid-cols-2 gap-x-6 gap-y-4">
            <div className="flex items-center text-gray-600">
              <FiBook className="mr-2 text-lg" />
              Title:
            </div>
            <div className="text-gray-800">{title}</div>

            <div className="flex items-center text-gray-600">
              <FiUser className="mr-2 text-lg" />
              Author:
            </div>
            <div className="text-gray-800">{author}</div>

            <div className="flex items-center text-gray-600">
              <FiTag className="mr-2 text-lg" />
              Category:
            </div>
            <div>
              <Tag color="blue">{categoryName}</Tag>
            </div>

            <div className="flex items-center text-gray-600">
              <FiUser className="mr-2 text-lg" />
              Requester:
            </div>
            <div className="text-gray-800">{requesterName}</div>

            <div className="flex items-center text-gray-600">
              <FiUserCheck className="mr-2 text-lg" />
              Approver:
            </div>
            <div className="text-gray-800">{approverName}</div>

            <div className="flex items-center text-gray-600">
              <FiCalendar className="mr-2 text-lg" />
              Due Date:
            </div>
            <div>
              {dayjs(dueDate).isBefore(dayjs().startOf("day")) ? (
                <Tag color="gray">Overdue</Tag>
              ) : (
                <Tag color="green">{dayjs(dueDate).format("DD/MM/YYYY")}</Tag>
              )}
            </div>

            <div className="flex items-center text-gray-600">
              <FiCalendar className="mr-2 text-lg" />
              Extended Due Date:
            </div>
            <div className="text-gray-800">
              {extendDueDate ? (
                <Tag color="yellow">
                  {dayjs(extendDueDate).format("DD/MM/YYYY")}
                </Tag>
              ) : (
                ""
              )}
            </div>
            <div className="flex items-center text-gray-600">
              <FiRepeat className="mr-2 text-lg" />
              Times Extended:
            </div>
            <div className="text-gray-800">
              {extendDueDateTimes ===
              validationData.bookBorrowedExtendDueDate ? (
                <Tag color="red">Max</Tag>
              ) : (
                <Tag color="green">{extendDueDateTimes}</Tag>
              )}
            </div>

            <div className="flex items-center text-gray-600">
              <FiEdit3 className="mr-2 text-lg" />
              Noted:
            </div>
            <div className="text-gray-800">{noted || ""}</div>
          </div>
        </div>
      </div>
    </Modal>
  );
};

export default BookBorrowedDetail;
