import {Layout} from 'antd'
import "../../assets/styles/AppContentStyle.css";
const {Content} = Layout;
import {Outlet} from "react-router";


const AppContent = () => {
    return (<>
        <Content className="app-content">
            <Outlet/>
        </Content>
    </>)
}

export default AppContent;