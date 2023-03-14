export default function makeCrudRoutes({
  contentType,
  components: { FormContainer, ListingContainer }
}) {
  return [
    {
      path: `/${contentType.name}/list`, 
      component: ListingContainer,
      props: {
        contentType
      }
    },
    {
      path: `/${contentType.name}/:id/edit`,
      component: FormContainer,
      props: route => ({ contentType, id: route.params.id })
    }
  ];
}
