import axios from 'axios'
export const HTTP = axios.create({
    //baseURL: 'http://10.32.4.127:8081/api/',
    baseURL: 'http://localhost:5001/api/',
    headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
    },
})

export const reLoadHTTP = () => {
    HTTP.defaults.headers = {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
    }
}

export const API_KEY = '?private_token=jZ5_H54HW4NPRF8s3mEQ'
export const API_KEY_1 = '&private_token=jZ5_H54HW4NPRF8s3mEQ'
export const PRIVATE_TOKEN = 'private_token=jZ5_H54HW4NPRF8s3mEQ'

export const SEARCH = (search) => `search?scope=projects&search=${search}&` + PRIVATE_TOKEN
export const GET_ALL_ISSUE = (id, pageSize, page) =>
    `projects/${id}/issues?per_page=${pageSize}&page=${page}` + API_KEY_1

export const PROJECT_MEMBER = (id) => `projects/${id}/members/all` + API_KEY
export const KEY_MAP = (id) => `projects/${id}/issues` + API_KEY
export const KEY_PROJECT = (id) => `projects/${id}` + API_KEY

export const GET_ALL_PROJECT = (pageSize, page) => `projects?per_page=${pageSize}&page=${page}` + API_KEY_1

export const SORT_BY_MONTH = (value, pageSize, page) =>
    `projects?orderby=id&last_activity_after=${value}&per_page=${pageSize}&page=${page}${API_KEY_1}`

export const GET_ALL_USERS = (pageSize, page) => `users?per_page=${pageSize}&page=${page}${API_KEY_1}`

export const HTTP_API_GITLAB = axios.create({
    baseURL: 'http://10.32.4.160:3080/api/v4/',
    headers: {
        'Content-Type': 'application/json',
    },
})

export const ENDPIONTS = {
    LEAVEOFF_REGISTER_LIST: 'leaveOff/getAllLeaveOff',
    SEARCH_REGISTER_LIST: 'leaveOff/GetByNameStatusDate',
    ADD_NEW_LEAVE_OFF: 'leaveOff/addNewLeaveOff',
}

export const GET_USER_BY_ID = (id) => `Users/getUserById/${id}`

export const ACCEPT_LEAVE_OFF = (id) => `leaveOff/accceptLeaveOff/${id}`
export const NOT_ACCEPT_LEAVE_OFF = (id) => `leaveOff/notAcceptLeaveOff/${id}`
export const GET_LEAVEOFF_BY_STATUS = (value) => `leaveOff/getAllLeaveOffByStatus?statusLO=${value}`
export const GET_LIST_LEAVEOFF_BY_USER = (id) => `leaveOff/getAllLeaveOffOfidUserByStatus/${id}`
export const GET_LEAVE_OFF_BY_ID = (id) => `leaveOff/getLeaveOffById/${id}`
export const UPDATE_LEAVE_OFF = (id) => `leaveOff/editRegisterLeaveOff/${id}`
export const DELETE_LEAVE_OFF = (id) => `leaveOff/deleteLeaveOffById/${id}`

export const HTTP_LOCAL = axios.create({
    baseURL: 'http://localhost:5001/api/',
    headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
    },
})
export const GET_GITLAB_PROJECT = `Project/getProjectOnGitlab`
export const GET_GITLAB_PROJECT_BY_DATE = (value) => `Project/getProjectGitLabByDayAfter?d=${value}`
export const GET_BY_YEAR = `leaveOff/GetAllLeaveOffByYear`
export const GET_LIST_PAID = `paid/`
export const GET_ALL_PROJECT_DATABASE = `Project/getAllProject`
export const GET_ALL_USERS_DATABASE = `Users/getAll`
export const GET_PROJECT_BY_ID = (id) => `Project/getById/${id}`
export const GET_GROUP_BY_ID = (id) => `/Group/getUserByGroup/${id}`
