export const filterOptionLabel = (input: string, option: any) => {
  return (
    option.label?.toLowerCase().indexOf(input.toLowerCase()) >= 0 ||
    option.key?.toString().toLowerCase().indexOf(input.toLowerCase()) >= 0 ||
    option.value?.toString().toLowerCase().indexOf(input.toLowerCase()) >= 0
  );
};
