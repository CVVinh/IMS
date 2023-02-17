import { components as layouts } from '@/layouts/components'

const components = [...layouts]

export const registration = (app) => {
    components.forEach((component) => app.component(component.__name, component))
}
