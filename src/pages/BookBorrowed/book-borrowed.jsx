import { Card, Table, Tooltip, Tag } from "antd";
import dayjs from "dayjs";
import { useEffect, useState } from "react";
import { defaultQueryParameters } from "../../constants/queryParameters";
import { useUserContext } from "../../routes/ProtectedRoute";
import { environment } from "../../constants/environment";
import { booksBorrowingRequestDetailServices } from "../../services/booksBorrowingRequestDetailServices";
import { userServices } from "../../services/userServices";
import { FiCheck, FiX, FiEye, FiPlusCircle } from "react-icons/fi";
import ExtendDueDateModal from "../../components/ui/ExtendDueDateModal";
import { validationData } from "../../constants/validationData";
import BookBorrowedDetail from "./book-borrowed-detail";
const { Column } = Table;

const BookBorrowed = () => {
  const [queryParameters, setQueryParameters] = useState(
    defaultQueryParameters
  );
  const [isExtendDueDate, setIsExtendDueDate] = useState(false);
  const [isViewDetail, setIsViewDetail] = useState(false);
  const [selectedBookBorrowedId, setSelectedBookBorrowedId] = useState(null);
  const { roleName, id } = useUserContext();
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const today = dayjs().startOf("day");
  const fetchData = () => {
    setLoading(true);
    if (roleName == environment.adminRole) {
      booksBorrowingRequestDetailServices
        .getsAsync(queryParameters)
        .then((res) => {
          setData([...res.items]);
          setQueryParameters({
            ...queryParameters,
            totalCount: res.totalCount,
          });
          setLoading(false);
        })
        .catch(() => setLoading(false));
    } else {
      userServices
        .getBookBorrowed(id, queryParameters)
        .then((res) => {
          setData([...res.items]);
          setQueryParameters({
            ...queryParameters,
            totalCount: res.totalCount,
          });
          setLoading(false);
        })
        .catch(() => setLoading(false));
    }
  };
  useEffect(() => {
    fetchData();
  }, [
    queryParameters.pageIndex,
    queryParameters.pageSize,
    queryParameters.search,
  ]);
  const handlePageChange = (pageIndex, pageSize) => {
    setQueryParameters({
      ...queryParameters,
      pageIndex: pageIndex,
      pageSize: pageSize,
    });
  };
  const handleOnSubmitExtendDueDate = (dueDate, bookBorrowedId) => {
    userServices
      .extendDueDate(id, {
        bookBorrowedDetailId: bookBorrowedId,
        extendDueDate: dueDate,
      })
      .then(() => {
        fetchData();
      });
  };
  const handleOnCancelExtendDueDate = () => {
    setIsExtendDueDate(false);
    setSelectedBookBorrowedId(null);
  };
  const handleOnClickExtendDueDate = (record) => {
    setSelectedBookBorrowedId(record.id);
    setIsExtendDueDate(true);
  };
  const handleOnCancelViewDetail = () => {
    setIsViewDetail(false);
    setSelectedBookBorrowedId(null);
  };
  const handleOnClickViewDetail = (record) => {
    setSelectedBookBorrowedId(record.id);
    setIsViewDetail(true);
  };
  const handleOnClickApproveExtendDueDate = (id) => {
    booksBorrowingRequestDetailServices
      .adjustExtendDueDateRequest(id, {
        status: 1,
      })
      .then(() => {
        fetchData();
      });
  };
  const handleOnClickRejectExtendDueDate = (id) => {
    booksBorrowingRequestDetailServices
      .adjustExtendDueDateRequest(id, {
        status: 0,
      })
      .then(() => {
        fetchData();
      });
  };
  return (
    <>
      {isExtendDueDate && (
        <ExtendDueDateModal
          bookBorrowedId={selectedBookBorrowedId}
          visible={isExtendDueDate}
          onCancel={handleOnCancelExtendDueDate}
          onExtend={handleOnSubmitExtendDueDate}
          currentDueDate={
            data.find((d) => d.id === selectedBookBorrowedId)?.dueDate
          }
        />
      )}
      {isViewDetail && (
        <BookBorrowedDetail
          visible={isViewDetail}
          onCancel={handleOnCancelViewDetail}
          onApprove={handleOnClickApproveExtendDueDate}
          onReject={handleOnClickRejectExtendDueDate}
          data={data.find((d) => d.id === selectedBookBorrowedId)}
          roleName={roleName}
          userId={id}
        />
      )}
      <Card
        title="Borrowed Book"
        className="book-list-card be-vietnam-pro-regular overflow-x-hidden overflow-x-scroll"
      >
        <Table
          bordered
          loading={loading}
          dataSource={data}
          pagination={{
            current: queryParameters.pageIndex,
            total: queryParameters.totalCount,
            pageSize: queryParameters.pageSize,
            showSizeChanger: true,
            pageSizeOptions: [5, 10, 20, 50],
            showTotal: (total, [start, end]) =>
              `From ${start} to ${end} items of ${total}`,
            onChange: (p, ps) => {
              handlePageChange(p, ps);
            },
          }}
        >
          <Column title="Title" dataIndex={["book", "title"]} key="bookTitle" />

          <Column
            title="Author"
            dataIndex={["book", "author"]}
            key="bookAuthor"
          />
          <Column
            title="Requester"
            dataIndex="requesterName"
            key="requesterName"
          />
          <Column
            title="Approver"
            dataIndex="approverName"
            key="approverName"
          />
          <Column
            title="Due Date"
            dataIndex="dueDate"
            key="dueDate"
            width={80}
            render={(dueDate, record) => {
              const date = dayjs(dueDate);
              const today = dayjs().startOf("day");

              if (date.isBefore(today, "day")) {
                return <Tag color="gray">Overdue</Tag>;
              }

              return <Tag color="green">{date.format("DD/MM/YYYY")}</Tag>;
            }}
          />
          <Column
            title="Extended Due Date"
            dataIndex="extendDueDate"
            key="extendDueDate"
            width={80}
            render={(extDate) => {
              if (!extDate) return "";
              return (
                <Tag color="yellow">{dayjs(extDate).format("DD/MM/YYYY")}</Tag>
              );
            }}
          />
          <Column
            title="Extended Due Date Times"
            dataIndex="extendDueDateTimes"
            key="extendDueDateTimes"
            className="!w-fit"
            width={120}
            align="center"
            render={(value) => {
              if (value == validationData.bookBorrowedExtendDueDate)
                return <Tag color="red">Max</Tag>;
              return <Tag color="green">{value}</Tag>;
            }}
          />
          <Column
            key="action"
            title="Action"
            minWidth={100}
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            render={(_, record) => {
              return (
                <div className="flex gap-2">
                  {!!record.extendDueDate &&
                    roleName === environment.adminRole && (
                      <Tooltip title="Approve extend due date">
                        <button
                          onClick={() => {
                            handleOnClickApproveExtendDueDate(record.id);
                          }}
                          className="p-2 rounded-lg bg-green-300 text-green-800 hover:bg-green-400 cursor-pointer"
                        >
                          <FiCheck size={20} />
                        </button>
                      </Tooltip>
                    )}

                  {!!record.extendDueDate &&
                    roleName === environment.adminRole && (
                      <Tooltip title="Reject extend due date">
                        <button
                          onClick={() => {
                            handleOnClickRejectExtendDueDate(record.id);
                          }}
                          className="p-2 rounded-lg bg-red-300 text-red-800 hover:bg-red-400 cursor-pointer"
                        >
                          <FiX size={20} />
                        </button>
                      </Tooltip>
                    )}
                  <Tooltip title="Detail" className="cursor-pointer">
                    <button
                      onClick={() => {
                        handleOnClickViewDetail(record);
                      }}
                      className="p-2 rounded-lg bg-emerald-300 text-emerald-800 hover:bg-emerald-400 cursor-pointer"
                    >
                      <FiEye size={20} />
                    </button>
                  </Tooltip>
                  {record.extendDueDateTimes <
                    validationData.bookBorrowedExtendDueDate &&
                    roleName !== environment.adminRole && (
                      <Tooltip title="Extend Due Date">
                        <button
                          onClick={() => handleOnClickExtendDueDate(record)}
                          className="
                        p-2 
                        rounded-lg 
                        bg-blue-600 
                        text-white 
                        hover:bg-blue-700 
                        focus:outline-none 
                        focus:ring-2 
                        focus:ring-blue-400
                        "
                        >
                          <FiPlusCircle size={20} />
                        </button>
                      </Tooltip>
                    )}
                </div>
              );
            }}
          />
        </Table>
      </Card>
    </>
  );
};

export default BookBorrowed;
