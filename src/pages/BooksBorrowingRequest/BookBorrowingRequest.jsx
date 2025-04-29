import { environment } from "../../constants/environment";
import { useUserContext } from "../../routes/ProtectedRoute";
import { Table, Card, Tooltip } from "antd";
import { FiCheck, FiX, FiEye } from "react-icons/fi";
import { useState, useEffect } from "react";
import { defaultQueryParameters } from "../../constants/queryParameters";
import { booksBorrowingRequestServices } from "../../services/booksBorrowingRequestServices";
import { userServices } from "../../services/userServices";
import dayjs from "dayjs";
import BookBorrowingDetail from "./BookBorrowingDetail";
import { booksBorrowingRequestStatus } from "../../constants/booksBorrowingRequestStatus";
const { Column } = Table;
const BookBorrowingRequest = () => {
  const { roleName, id, firstName, lastName } = useUserContext();
  const [queryParameters, setQueryParameters] = useState({
    ...defaultQueryParameters,
    status: "111",
    fromRequestedDate: dayjs("2000-04-26").format("YYYY-MM-DD"),
    toRequestedDate: dayjs().format("YYYY-MM-DD"),
  });
  const [showDetailRequest, setShowDetailRequest] = useState(false);
  const [selectedDetailRequest, setSelectedDetailRequest] = useState(null);
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const fetchData = (isSetData = true) => {
    setLoading(true);
    if (roleName === environment.adminRole) {
      booksBorrowingRequestServices
        .gets(queryParameters)
        .then((res) => {
          setData(res.items);
          setQueryParameters({
            ...queryParameters,
            totalCount: res.totalCount,
          });
          setLoading(false);
        })
        .catch(() => setLoading(false));
    } else
      userServices
        .getBookBorrowingRequests(queryParameters, id)
        .then((res) => {
          setData(res.items);
          setQueryParameters({
            ...queryParameters,
            totalCount: res.totalCount,
          });
          setLoading(false);
        })
        .catch(() => setLoading(false));
  };
  useEffect(() => {
    let isSetData = true;
    fetchData(isSetData);
    return () => {
      isSetData = false;
    };
  }, [
    queryParameters.pageIndex,
    queryParameters.pageSize,
    queryParameters.totalCount,
    queryParameters.search,
  ]);
  const handlePageChange = (pageIndex, pageSize) => {
    setQueryParameters({
      ...queryParameters,
      pageIndex: pageIndex,
      pageSize: pageSize,
    });
  };
  const handleOnUpdateRequestStatus = (id, status) => {
    setSelectedDetailRequest(null);
    setShowDetailRequest(false);
    setLoading(true);
    booksBorrowingRequestServices
      .changeStatus({
        id: id,
        status: status,
      })
      .then(() => {
        fetchData()
          .then(() => {
            setLoading(false);
          })
          .catch(() => {
            setLoading(false);
          });
      })
      .catch(() => setLoading(false));
  };
  const handleOnApproveRequest = (id) => {
    handleOnUpdateRequestStatus(id, booksBorrowingRequestStatus.Approved);
  };
  const handleOnRejectRequest = (id) => {
    handleOnUpdateRequestStatus(id, booksBorrowingRequestStatus.Rejected);
  };
  const handleOnViewDetailRequest = (id) => {
    setSelectedDetailRequest(id);
    setShowDetailRequest(true);
  };
  const handleCancelViewDetailRequest = () => {
    setSelectedDetailRequest(null);
    setShowDetailRequest(false);
  };
  return (
    <>
      {showDetailRequest && (
        <BookBorrowingDetail
          bookBorrowingId={selectedDetailRequest}
          onApprove={handleOnApproveRequest}
          onReject={handleOnRejectRequest}
          onCancel={handleCancelViewDetailRequest}
        />
      )}
      <Card
        title="Books borrowing"
        className="book-list-card be-vietnam-pro-regular"
      >
        <Table
          bordered
          className="overflow-x-scroll"
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
          {roleName == environment.adminRole && (
            <Column
              onHeaderCell={() => ({
                className: "be-vietnam-pro-medium",
              })}
              title="Requester"
              key="requester"
              dataIndex={["requester", "fullName"]}
            />
          )}
          <Column
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            title="Approver"
            key="approver"
            dataIndex={["approver", "fullName"]}
          />
          <Column
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            title="Date Requested"
            key="dateRequested"
            dataIndex="dateRequested"
          />
          <Column
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            title="Date Approved"
            key="dateApproved"
            dataIndex="dateApproved"
          />
          <Column
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            title="Number of books"
            key="booksBorrowingNumber"
            dataIndex="booksBorrowingNumber"
            width={170}
            align={"center"}
            render={(booksBorrowingNumber) => (
              <div className="inline-block px-3 py-1 rounded-lg bg-blue-200 text-blue-800 font-medium">
                {booksBorrowingNumber}
              </div>
            )}
          />
          <Column
            title="Status"
            key="status"
            align={"center"}
            dataIndex="status"
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            render={(status) => {
              let bgClass = "";
              let text = "";

              switch (status) {
                case 0:
                  bgClass = "bg-yellow-200 text-yellow-800";
                  text = "Waiting";
                  break;
                case 1:
                  bgClass = "bg-green-300 text-green-800";
                  text = "Approved";
                  break;
                case 2:
                  bgClass = "bg-red-300 text-red-800";
                  text = "Rejected";
                  break;
                case 3:
                  bgClass = "bg-gray-200 text-gray-800";
                  text = "Overdue";
                  break;
                default:
                  bgClass = "bg-gray-200 text-gray-800";
                  text = "Unknown";
              }

              return (
                <div
                  className={`px-3 py-1 rounded-lg font-medium inline-block ${bgClass}`}
                >
                  {text}
                </div>
              );
            }}
          />

          <Column
            key="action"
            title="Action"
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            render={(_, record) => (
              <div className="flex gap-2">
                {record.status === 0 && roleName === environment.adminRole && (
                  <Tooltip title="Approve">
                    <button
                      onClick={() => handleOnApproveRequest(record.id)}
                      className="p-2 rounded-lg bg-green-300 text-green-800 hover:bg-green-400 cursor-pointer"
                    >
                      <FiCheck size={20} />
                    </button>
                  </Tooltip>
                )}

                {record.status === 0 && roleName === environment.adminRole && (
                  <Tooltip title="Reject">
                    <button
                      onClick={() => handleOnRejectRequest(record.id)}
                      className="p-2 rounded-lg bg-red-300 text-red-800 hover:bg-red-400 cursor-pointer"
                    >
                      <FiX size={20} />
                    </button>
                  </Tooltip>
                )}
                <Tooltip title="Detail" className="cursor-pointer">
                  <button
                    onClick={() => handleOnViewDetailRequest(record.id)}
                    className="p-2 rounded-lg bg-emerald-300 text-emerald-800 hover:bg-emerald-400 cursor-pointer"
                  >
                    <FiEye size={20} />
                  </button>
                </Tooltip>
              </div>
            )}
          />
        </Table>
      </Card>
    </>
  );
};

export default BookBorrowingRequest;
