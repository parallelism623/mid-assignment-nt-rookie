import React, { useEffect, useState } from "react";
import { Modal, Table, Tooltip, Tag } from "antd";
import { FiCheck, FiX, FiSlash } from "react-icons/fi";
import { booksBorrowingRequestServices } from "../../services/booksBorrowingRequestServices";
import { booksBorrowingRequestStatus } from "../../constants/booksBorrowingRequestStatus";
import dayjs from "dayjs";
const { Column } = Table;
const BookBorrowingDetail = ({
  bookBorrowingId,
  onCancel,
  onApprove,
  onReject,
}) => {
  const [booksBorrow, setBooksBorrow] = useState([]);
  const [requestBorrowingStatus, setRequestBorrowingStatus] = useState(null);
  const [loading, setLoading] = useState(false);

  const fetchData = (isSetData = true) => {
    setLoading(true);
    booksBorrowingRequestServices
      .getDetail(bookBorrowingId)
      .then((res) => {
        setRequestBorrowingStatus(res.status);
        setBooksBorrow([
          ...res.items.map((p) => {
            return { ...p };
          }),
        ]);
        setLoading(false);
      })
      .catch(() => setLoading(false));
  };
  useEffect(() => {
    let isSetDate = true;
    fetchData(isSetDate);
    return () => {
      isSetDate = false;
    };
  }, [bookBorrowingId]);
  return (
    <>
      <Modal
        onCancel={onCancel}
        className="book-borrow-modal"
        title={
          <>
            <h2>Book borrowing detail</h2>
          </>
        }
        open={true}
        footer={[
          <div
            className="flex gap-3 justify-end"
            key="footer-modal-book-borrowing-request-detail"
          >
            {requestBorrowingStatus === booksBorrowingRequestStatus.Waiting && (
              <Tooltip title="Approve" key="approve">
                <button
                  onClick={() => onApprove(bookBorrowingId)}
                  className="p-2 rounded-lg bg-green-300 text-green-800 hover:bg-green-400"
                >
                  <FiCheck size={20} />
                </button>
              </Tooltip>
            )}
            {requestBorrowingStatus === booksBorrowingRequestStatus.Waiting && (
              <Tooltip title="Reject" key="reject">
                <button
                  onClick={() => onReject(bookBorrowingId)}
                  className="p-2 rounded-lg bg-red-300 text-red-800 hover:bg-red-400"
                >
                  <FiX size={20} />
                </button>
              </Tooltip>
            )}
          </div>,
        ]}
      >
        <Table
          dataSource={booksBorrow}
          pagination={false}
          loading={loading}
          onChange={(e) => {
            setBooksBorrow([...e]);
          }}
        >
          <Column title="Title" dataIndex="title" key="title" />
          <Column title="Author" dataIndex="author" key="author" />
          <Column
            title="Category"
            dataIndex={["category", "name"]}
            key="categoryName"
          />
          <Column
            title="Due date"
            dataIndex="dueDate"
            key="dueDate"
            render={(dueDate, record) => {
              const date = dayjs(dueDate);
              const today = dayjs().startOf("day");

              if (date.isBefore(today, "day")) {
                return (
                  <span className="bg-gray-200 p-1 rounded-xl">Overdue</span>
                );
              }

              return (
                <span className="bg-green-200 p-1 rounded-xl">
                  {date.format("DD/MM/YYYY")}
                </span>
              );
            }}
          />
          <Column title="Noted" dataIndex="noted" key="noted" ellipsis></Column>
        </Table>
      </Modal>
    </>
  );
};

export default BookBorrowingDetail;
