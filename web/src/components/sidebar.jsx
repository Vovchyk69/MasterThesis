import {
  ProSidebar,
  Menu,
  MenuItem,
  SidebarHeader,
  SidebarContent,
} from 'react-pro-sidebar'
import { FaTachometerAlt, FaSchool, FaUser, FaSignOutAlt } from 'react-icons/fa'
import 'react-pro-sidebar/dist/css/styles.css'
import { Link } from 'react-router-dom'
import React, { Component } from 'react'
import UserFront from '@userfront/react'

export class Sidebar extends Component {
  render() {
    return (
      <div
        style={{
          height: '100vh',
          width: '120px',
          zIndex: 1,
          top: 0,
          left: 0,
          position: 'fixed',
        }}
      >
        <ProSidebar
          image={false}
          collapsed={this.props.collapsed || true}
          toggled={this.props.toggled || true}
          breakPoint="md"
          onToggle={undefined}
        >
          <SidebarHeader>
            <div
              style={{
                padding: '24px',
                textTransform: 'uppercase',
                fontWeight: 'bold',
                fontSize: 14,
                letterSpacing: '1px',
                overflow: 'hidden',
                textOverflow: 'ellipsis',
                whiteSpace: 'nowrap',
              }}
            >
              SCH
            </div>
          </SidebarHeader>

          <SidebarContent>
            <Menu iconShape="circle" placeholder="Scheduler">
              {!UserFront.accessToken() && (
                <MenuItem icon={<FaUser />}>
                  <Link to="/login" />
                </MenuItem>
              )}
              <MenuItem icon={<FaTachometerAlt />}>
                <Link to="/calendar" />
              </MenuItem>
              <MenuItem icon={<FaSchool />}>
                <Link to="/home" />
              </MenuItem>
              {UserFront.accessToken() && (
                <MenuItem
                  icon={<FaSignOutAlt />}
                  onClick={UserFront.logout}
                ></MenuItem>
              )}
            </Menu>
          </SidebarContent>
        </ProSidebar>
      </div>
    )
  }
}
