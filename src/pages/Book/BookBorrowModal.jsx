import React, { useEffect, useState } from "react";
import { Input, Modal, Card, Table, DatePicker } from "antd";
import { bookServices } from "../../services/bookServices";
import dayjs from "dayjs";
import { useUserContext } from "../../routes/ProtectedRoute";
import { userServices } from "../../services/userServices";
import { validationData } from "../../constants/validationData";
const { Column } = Table;
const BookBorrowModal = ({
  isModalOpen,
  setIsModalOpen,
  bookIds,
  setSelectBookIds,
  onSubmit,
}) => {
  const [booksBorrow, setBooksBorrow] = useState([]);
  const [loading, setLoading] = useState(false);
  const { id, bookBorrowingLimit } = useUserContext();
  const handleOk = () => {
    const bookBorrowingRequest = getBookBorrowingRequest(id, booksBorrow);
    userServices.createBookBorrowingRequest(bookBorrowingRequest).then(() => {
      setIsModalOpen(false);
      setSelectBookIds([]);
      onSubmit();
    });
  };

  const handleCancel = () => {
    setIsModalOpen(false);
    setSelectBookIds([]);
  };
  useEffect(() => {
    setLoading(true);
    bookServices
      .gets({ ids: bookIds })
      .then((res) => {
        setBooksBorrow([
          ...res.items.map((p) => {
            return { ...p, dueDate: dayjs().format("YYYY-MM-DD") };
          }),
        ]);
        setLoading(false);
      })
      .catch(() => setLoading(false));
  }, [bookIds]);
  const handleOnRecordChange = (record) => {
    const newBooksBorrow = booksBorrow.map((item) =>
      item.id === record.id ? record : item
    );
    setBooksBorrow(newBooksBorrow);
  };
  return (
    <>
      <Modal
        className="book-borrow-modal"
        title={
          <>
            <h2>Book borrowing</h2>
            <div className="flex flex-col">
              <span
                className={`text-sm ${
                  bookBorrowingLimit > 0 ? "text-green-400" : "text-red-400"
                }`}
              >{`You have ${bookBorrowingLimit} book rentals left`}</span>
            </div>
          </>
        }
        open={isModalOpen}
        onOk={handleOk}
        onCancel={handleCancel}
        okButtonProps={{
          disabled: bookBorrowingLimit === 0,
        }}
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
            render={(value, record) => {
              return (
                <DatePicker
                  disabledDate={(current) => {
                    return current && current < dayjs().startOf("day");
                  }}
                  format="YYYY-MM-DD"
                  value={
                    record.dueDate ? dayjs(record.dueDate, "YYYY-MM-DD") : null
                  }
                  onChange={(date, dateString) => {
                    record.dueDate = dateString;
                    handleOnRecordChange(record);
                  }}
                  allowClear={false}
                />
              );
            }}
          />
          <Column
            title={`Noted (${validationData.bookBorrowingDetailNotedMaxLength} characters)`}
            dataIndex="noted"
            key="noted"
            render={(value, record) => (
              <Input
                maxLength={2000}
                value={value}
                onChange={(e) => {
                  record.noted = e.target.value;
                  handleOnRecordChange(record);
                }}
                placeholder="Enter note..."
              />
            )}
          ></Column>
        </Table>
      </Modal>
    </>
  );
};

export default BookBorrowModal;

const getBookBorrowingRequest = (userId, booksSelected) => {
  const dateRequested = dayjs().format("YYYY-MM-DD");
  const requesterId = userId;
  const borrowingRequestDetails = booksSelected.map((b) => {
    return { bookId: b.id, dueDate: b.dueDate, noted: b.noted ?? "" };
  });
  return {
    dateRequested: dateRequested,
    requesterId: requesterId,
    borrowingRequestDetails: borrowingRequestDetails,
  };
};
